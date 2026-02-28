IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203205608_fix task model3'
)
BEGIN
    CREATE TABLE [Admins] (
        [Id] nvarchar(255) NOT NULL,
        [Email] nvarchar(max) NOT NULL,
        [Password] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Admins] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203205608_fix task model3'
)
BEGIN
    CREATE TABLE [Communities] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [Image] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Communities] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203205608_fix task model3'
)
BEGIN
    CREATE TABLE [Sponsors] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [CompanyName] nvarchar(max) NOT NULL,
        [ContactInfo] nvarchar(max) NOT NULL,
        [Logo] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Sponsors] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203205608_fix task model3'
)
BEGIN
    CREATE TABLE [Users] (
        [Id] nvarchar(255) NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Email] nvarchar(max) NOT NULL,
        [Password] nvarchar(max) NOT NULL,
        [Bio] nvarchar(max) NOT NULL,
        [Profile_Image] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203205608_fix task model3'
)
BEGIN
    CREATE TABLE [Assistants] (
        [Id] nvarchar(255) NOT NULL,
        [University_Name] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Assistants] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Assistants_Users_Id] FOREIGN KEY ([Id]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203205608_fix task model3'
)
BEGIN
    CREATE TABLE [Doctors] (
        [Id] nvarchar(255) NOT NULL,
        [Specialization] nvarchar(max) NOT NULL,
        [University_Name] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Doctors] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Doctors_Users_Id] FOREIGN KEY ([Id]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203205608_fix task model3'
)
BEGIN
    CREATE TABLE [Posts] (
        [Id] int NOT NULL IDENTITY,
        [Content] nvarchar(max) NOT NULL,
        [Status] int NOT NULL,
        [AdminId] nvarchar(255) NOT NULL,
        [CommunityId] int NOT NULL,
        [UserId] nvarchar(255) NOT NULL,
        CONSTRAINT [PK_Posts] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Posts_Admins_AdminId] FOREIGN KEY ([AdminId]) REFERENCES [Admins] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Posts_Communities_CommunityId] FOREIGN KEY ([CommunityId]) REFERENCES [Communities] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Posts_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203205608_fix task model3'
)
BEGIN
    CREATE TABLE [Students] (
        [Id] nvarchar(255) NOT NULL,
        [GraduationProjectDatails] nvarchar(max) NOT NULL,
        [Department] nvarchar(max) NOT NULL,
        [Skills] nvarchar(max) NOT NULL,
        [University_Name] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Students] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Students_Users_Id] FOREIGN KEY ([Id]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203205608_fix task model3'
)
BEGIN
    CREATE TABLE [Projects] (
        [Id] int NOT NULL IDENTITY,
        [Title] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [TechnologyUsed] nvarchar(max) NOT NULL,
        [ProjectFilePath] nvarchar(max) NOT NULL,
        [AssignedDoctorId] nvarchar(255) NULL,
        [AssignedAssistantId] nvarchar(255) NULL,
        CONSTRAINT [PK_Projects] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Projects_Assistants_AssignedAssistantId] FOREIGN KEY ([AssignedAssistantId]) REFERENCES [Assistants] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Projects_Doctors_AssignedDoctorId] FOREIGN KEY ([AssignedDoctorId]) REFERENCES [Doctors] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203205608_fix task model3'
)
BEGIN
    CREATE TABLE [Comments] (
        [Id] int NOT NULL IDENTITY,
        [Content] nvarchar(max) NOT NULL,
        [PostId] int NOT NULL,
        CONSTRAINT [PK_Comments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Comments_Posts_PostId] FOREIGN KEY ([PostId]) REFERENCES [Posts] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203205608_fix task model3'
)
BEGIN
    CREATE TABLE [Tasks] (
        [Id] int NOT NULL IDENTITY,
        [Details] nvarchar(max) NOT NULL,
        [Deadline] datetime2 NOT NULL,
        [Status] nvarchar(500) NOT NULL,
        [SolutionFile] nvarchar(max) NOT NULL,
        [AssignedStudentId] nvarchar(255) NULL,
        [AssignedDoctorId] nvarchar(255) NULL,
        [AssignedAssistantId] nvarchar(255) NULL,
        [AssistantId] nvarchar(255) NULL,
        [DoctorId] nvarchar(255) NULL,
        [StudentId] nvarchar(255) NULL,
        CONSTRAINT [PK_Tasks] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Tasks_Assistants_AssignedAssistantId] FOREIGN KEY ([AssignedAssistantId]) REFERENCES [Assistants] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Tasks_Assistants_AssistantId] FOREIGN KEY ([AssistantId]) REFERENCES [Assistants] ([Id]),
        CONSTRAINT [FK_Tasks_Doctors_AssignedDoctorId] FOREIGN KEY ([AssignedDoctorId]) REFERENCES [Doctors] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Tasks_Doctors_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [Doctors] ([Id]),
        CONSTRAINT [FK_Tasks_Students_AssignedStudentId] FOREIGN KEY ([AssignedStudentId]) REFERENCES [Students] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Tasks_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203205608_fix task model3'
)
BEGIN
    CREATE TABLE [Conversations] (
        [Id] int NOT NULL IDENTITY,
        [Start_Date] datetime2 NOT NULL,
        [Sender_ID] nvarchar(255) NOT NULL,
        [TargetUser_ID] nvarchar(255) NOT NULL,
        [ProjectId] int NULL,
        [UserId] nvarchar(255) NULL,
        CONSTRAINT [PK_Conversations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Conversations_Projects_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [Projects] ([Id]),
        CONSTRAINT [FK_Conversations_Users_Sender_ID] FOREIGN KEY ([Sender_ID]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Conversations_Users_TargetUser_ID] FOREIGN KEY ([TargetUser_ID]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Conversations_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203205608_fix task model3'
)
BEGIN
    CREATE TABLE [ProjectSponsor] (
        [ProjectsId] int NOT NULL,
        [SponsorsId] int NOT NULL,
        CONSTRAINT [PK_ProjectSponsor] PRIMARY KEY ([ProjectsId], [SponsorsId]),
        CONSTRAINT [FK_ProjectSponsor_Projects_ProjectsId] FOREIGN KEY ([ProjectsId]) REFERENCES [Projects] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ProjectSponsor_Sponsors_SponsorsId] FOREIGN KEY ([SponsorsId]) REFERENCES [Sponsors] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203205608_fix task model3'
)
BEGIN
    CREATE TABLE [Teams] (
        [Id] int NOT NULL IDENTITY,
        [TeamName] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [DoctorId] nvarchar(255) NOT NULL,
        [ProjectId] int NOT NULL,
        CONSTRAINT [PK_Teams] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Teams_Doctors_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [Doctors] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Teams_Projects_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [Projects] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203205608_fix task model3'
)
BEGIN
    CREATE TABLE [Messages] (
        [Id] int NOT NULL IDENTITY,
        [Content] nvarchar(max) NOT NULL,
        [Timestamp] datetime2 NOT NULL,
        [Sender_ID] nvarchar(255) NOT NULL,
        [Conversation_ID] int NOT NULL,
        CONSTRAINT [PK_Messages] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Messages_Conversations_Conversation_ID] FOREIGN KEY ([Conversation_ID]) REFERENCES [Conversations] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Messages_Users_Sender_ID] FOREIGN KEY ([Sender_ID]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203205608_fix task model3'
)
BEGIN
    CREATE TABLE [Meetings] (
        [Id] int NOT NULL IDENTITY,
        [ScheduleTime] datetime2 NOT NULL,
        [ZoomLink] nvarchar(500) NOT NULL,
        [Details] nvarchar(max) NOT NULL,
        [DoctorId] nvarchar(255) NOT NULL,
        [TeamId] int NOT NULL,
        CONSTRAINT [PK_Meetings] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Meetings_Doctors_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [Doctors] ([Id]),
        CONSTRAINT [FK_Meetings_Teams_TeamId] FOREIGN KEY ([TeamId]) REFERENCES [Teams] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203205608_fix task model3'
)
BEGIN
    CREATE TABLE [StudentTeam] (
        [StudentsId] nvarchar(255) NOT NULL,
        [TeamsId] int NOT NULL,
        CONSTRAINT [PK_StudentTeam] PRIMARY KEY ([StudentsId], [TeamsId]),
        CONSTRAINT [FK_StudentTeam_Students_StudentsId] FOREIGN KEY ([StudentsId]) REFERENCES [Students] ([Id]),
        CONSTRAINT [FK_StudentTeam_Teams_TeamsId] FOREIGN KEY ([TeamsId]) REFERENCES [Teams] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203205608_fix task model3'
)
BEGIN
    CREATE INDEX [IX_Comments_PostId] ON [Comments] ([PostId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203205608_fix task model3'
)
BEGIN
    CREATE INDEX [IX_Conversations_ProjectId] ON [Conversations] ([ProjectId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203205608_fix task model3'
)
BEGIN
    CREATE INDEX [IX_Conversations_Sender_ID] ON [Conversations] ([Sender_ID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203205608_fix task model3'
)
BEGIN
    CREATE INDEX [IX_Conversations_TargetUser_ID] ON [Conversations] ([TargetUser_ID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203205608_fix task model3'
)
BEGIN
    CREATE INDEX [IX_Conversations_UserId] ON [Conversations] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203205608_fix task model3'
)
BEGIN
    CREATE INDEX [IX_Meetings_DoctorId] ON [Meetings] ([DoctorId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203205608_fix task model3'
)
BEGIN
    CREATE INDEX [IX_Meetings_TeamId] ON [Meetings] ([TeamId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203205608_fix task model3'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Meetings_ZoomLink] ON [Meetings] ([ZoomLink]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203205608_fix task model3'
)
BEGIN
    CREATE INDEX [IX_Messages_Conversation_ID] ON [Messages] ([Conversation_ID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203205608_fix task model3'
)
BEGIN
    CREATE INDEX [IX_Messages_Sender_ID] ON [Messages] ([Sender_ID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203205608_fix task model3'
)
BEGIN
    CREATE INDEX [IX_Posts_AdminId] ON [Posts] ([AdminId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203205608_fix task model3'
)
BEGIN
    CREATE INDEX [IX_Posts_CommunityId] ON [Posts] ([CommunityId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203205608_fix task model3'
)
BEGIN
    CREATE INDEX [IX_Posts_UserId] ON [Posts] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203205608_fix task model3'
)
BEGIN
    CREATE INDEX [IX_Projects_AssignedAssistantId] ON [Projects] ([AssignedAssistantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203205608_fix task model3'
)
BEGIN
    CREATE INDEX [IX_Projects_AssignedDoctorId] ON [Projects] ([AssignedDoctorId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203205608_fix task model3'
)
BEGIN
    CREATE INDEX [IX_ProjectSponsor_SponsorsId] ON [ProjectSponsor] ([SponsorsId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203205608_fix task model3'
)
BEGIN
    CREATE INDEX [IX_StudentTeam_TeamsId] ON [StudentTeam] ([TeamsId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203205608_fix task model3'
)
BEGIN
    CREATE INDEX [IX_Tasks_AssignedAssistantId] ON [Tasks] ([AssignedAssistantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203205608_fix task model3'
)
BEGIN
    CREATE INDEX [IX_Tasks_AssignedDoctorId] ON [Tasks] ([AssignedDoctorId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203205608_fix task model3'
)
BEGIN
    CREATE INDEX [IX_Tasks_AssignedStudentId] ON [Tasks] ([AssignedStudentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203205608_fix task model3'
)
BEGIN
    CREATE INDEX [IX_Tasks_AssistantId] ON [Tasks] ([AssistantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203205608_fix task model3'
)
BEGIN
    CREATE INDEX [IX_Tasks_DoctorId] ON [Tasks] ([DoctorId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203205608_fix task model3'
)
BEGIN
    CREATE INDEX [IX_Tasks_StudentId] ON [Tasks] ([StudentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203205608_fix task model3'
)
BEGIN
    CREATE INDEX [IX_Teams_DoctorId] ON [Teams] ([DoctorId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203205608_fix task model3'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Teams_ProjectId] ON [Teams] ([ProjectId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203205608_fix task model3'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260203205608_fix task model3', N'9.0.12');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203210447_fix project model1'
)
BEGIN
    ALTER TABLE [Tasks] DROP CONSTRAINT [FK_Tasks_Assistants_AssistantId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203210447_fix project model1'
)
BEGIN
    ALTER TABLE [Tasks] DROP CONSTRAINT [FK_Tasks_Doctors_DoctorId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203210447_fix project model1'
)
BEGIN
    ALTER TABLE [Tasks] DROP CONSTRAINT [FK_Tasks_Students_StudentId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203210447_fix project model1'
)
BEGIN
    DROP INDEX [IX_Tasks_AssistantId] ON [Tasks];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203210447_fix project model1'
)
BEGIN
    DROP INDEX [IX_Tasks_DoctorId] ON [Tasks];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203210447_fix project model1'
)
BEGIN
    DROP INDEX [IX_Tasks_StudentId] ON [Tasks];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203210447_fix project model1'
)
BEGIN
    DECLARE @var sysname;
    SELECT @var = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Tasks]') AND [c].[name] = N'AssistantId');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [Tasks] DROP CONSTRAINT [' + @var + '];');
    ALTER TABLE [Tasks] DROP COLUMN [AssistantId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203210447_fix project model1'
)
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Tasks]') AND [c].[name] = N'DoctorId');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Tasks] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [Tasks] DROP COLUMN [DoctorId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203210447_fix project model1'
)
BEGIN
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Tasks]') AND [c].[name] = N'StudentId');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Tasks] DROP CONSTRAINT [' + @var2 + '];');
    ALTER TABLE [Tasks] DROP COLUMN [StudentId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203210447_fix project model1'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260203210447_fix project model1', N'9.0.12');
END;

COMMIT;
GO