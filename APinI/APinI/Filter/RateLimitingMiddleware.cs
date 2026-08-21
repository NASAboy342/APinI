using System.Collections.Concurrent;
using System.Net;
using APinI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace APinI.Filter
{
    /// <summary>
    /// Blocks any client IP that calls an API more than 10 times within a single second.
    /// Once the threshold is reached, the IP is blocked for 1 day.
    /// Apply this middleware to all controllers via Program.cs.
    /// </summary>
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;

        // Max requests allowed per IP within a 1-second window.
        private const int MaxRequestsPerSecond = 5;

        // Duration an IP stays blocked once it exceeds the limit (1 day).
        private static readonly TimeSpan BlockDuration = TimeSpan.FromDays(1);

        // Sliding window used to count requests in the current second.
        private static readonly TimeSpan Window = TimeSpan.FromSeconds(1);

        // Per-IP sliding window of request timestamps.
        private static readonly ConcurrentDictionary<string, List<DateTime>> _requestWindow =
            new ConcurrentDictionary<string, List<DateTime>>();

        // Per-IP block-until timestamp (null = not blocked).
        private static readonly ConcurrentDictionary<string, DateTime> _blockUntil =
            new ConcurrentDictionary<string, DateTime>();

        // Background cleanup so stale entries don't grow memory indefinitely.
        private static readonly Timer _cleanupTimer = new Timer(DoCleanup);

        public RateLimitingMiddleware(RequestDelegate next)
        {
            _next = next;
            // Run cleanup every 5 minutes.
            _cleanupTimer.Change(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var clientIp = GetClientIp(context);
            var now = DateTime.UtcNow;

            // If the IP is currently blocked, short-circuit with 429.
            if (IsBlocked(clientIp, now))
            {
                await WriteBlockedResponseAsync(context, clientIp);
                return;
            }

            // Sliding window: drop timestamps older than 1 second and count this request.
            var timestamps = _requestWindow.GetOrAdd(clientIp, _ => new List<DateTime>());
            bool blocked = false;
            lock (timestamps)
            {
                // Remove entries older than the 1-second window.
                var cutoff = now - Window;
                while (timestamps.Count > 0 && timestamps[0] < cutoff)
                {
                    timestamps.RemoveAt(0);
                }

                timestamps.Add(now);

                if (timestamps.Count > MaxRequestsPerSecond)
                {
                    // Threshold exceeded -> block for 1 day.
                    _blockUntil[clientIp] = now + BlockDuration;
                    blocked = true;
                }
            }

            // Await outside the lock to avoid awaiting inside a lock statement.
            if (blocked)
            {
                await WriteBlockedResponseAsync(context, clientIp);
                return;
            }

            await _next(context);
        }

        private static bool IsBlocked(string clientIp, DateTime now)
        {
            if (_blockUntil.TryGetValue(clientIp, out var until))
            {
                if (until > now)
                {
                    return true;
                }

                // Block expired -> remove it so the IP can request again.
                _blockUntil.TryRemove(clientIp, out _);
            }

            return false;
        }

        private static async Task WriteBlockedResponseAsync(HttpContext context, string clientIp)
        {
            var response = new ApiBaseResponse<BaseResponse>(
                new BaseResponse
                {
                    ErrorCode = 429,
                    ErrorMessage = $"Rate limit exceeded. IP {clientIp} has been blocked for 1 day. (Max {MaxRequestsPerSecond} requests per second.)"
                });

            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }

        /// <summary>
        /// Extracts the real client IP, preferring the X-Forwarded-For header
        /// (set by proxies/load balancers) and falling back to the connection remote IP.
        /// </summary>
        private static string GetClientIp(HttpContext context)
        {
            var forwarded = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwarded))
            {
                // The first entry is the original client.
                return forwarded.Split(',')[0].Trim();
            }

            var ip = context.Connection.RemoteIpAddress;
            if (ip != null)
            {
                // Normalize IPv4-mapped IPv6 addresses (e.g. ::ffff:127.0.0.1 -> 127.0.0.1).
                if (ip.IsIPv4MappedToIPv6)
                {
                    ip = ip.MapToIPv4();
                }

                return ip.ToString();
            }

            return "unknown";
        }

        private static void DoCleanup(object? state)
        {
            var now = DateTime.UtcNow;

            // Clear expired blocks.
            foreach (var kv in _blockUntil)
            {
                if (kv.Value <= now)
                {
                    _blockUntil.TryRemove(kv.Key, out _);
                }
            }

            // Drop window entries whose last request is older than the window.
            foreach (var kv in _requestWindow)
            {
                lock (kv.Value)
                {
                    if (kv.Value.Count == 0 || kv.Value[kv.Value.Count - 1] < now - Window)
                    {
                        _requestWindow.TryRemove(kv.Key, out _);
                    }
                }
            }
        }
    }
}