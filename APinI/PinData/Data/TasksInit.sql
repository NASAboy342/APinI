
Declare @tasks table
(
	[Name] NVARCHAR(50) NOT NULL,
	[Description] NVARCHAR(50) NOT NULL,
	[Status] NVARCHAR(50) NOT NULL
)

insert into @tasks
(
	[Name],
	[Description],
	[Status]
)
Values
('Go for run','To self running', 'Todo'),
('Learn vue','To join pilot team', 'InProgress'),
('Write a blog post','To share my knowledge', 'Done'),
('Cook dinner','To feed my family', 'Todo'),
('Read a book','To relax and learn', 'InProgress'),
('Clean the house','To make it tidy', 'Done')

Merge [dbo].[Tasks] as T
using @tasks as S
on T.[Name] = S.[Name]
and T.[Description] = S.[Description]
WHEN NOT MATCHED BY TARGET THEN
	INSERT
	(	
		[Name],
		[Description],
		[Status]
	)
	VALUES
	(
		S.[Name],
		S.[Description],
		S.[Status]
	);
GO