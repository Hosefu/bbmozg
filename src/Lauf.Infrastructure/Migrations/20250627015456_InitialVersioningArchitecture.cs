using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lauf.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialVersioningArchitecture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Achievements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Rarity = table.Column<string>(type: "TEXT", nullable: false),
                    IconUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Achievements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Flows",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Tags = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false, defaultValue: "Draft"),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    IsRequired = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedById = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flows", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FlowVersions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OriginalId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "Идентификатор оригинального потока"),
                    Version = table.Column<int>(type: "INTEGER", nullable: false, comment: "Номер версии"),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false, comment: "Является ли версия активной"),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false, comment: "Название потока"),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false, comment: "Описание потока"),
                    Tags = table.Column<string>(type: "TEXT", nullable: false, defaultValue: "", comment: "Теги потока (разделенные запятыми)"),
                    Status = table.Column<string>(type: "TEXT", nullable: false, defaultValue: "Draft", comment: "Статус потока"),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false, comment: "Приоритет потока"),
                    IsRequired = table.Column<bool>(type: "INTEGER", nullable: false, comment: "Является ли поток обязательным"),
                    CreatedById = table.Column<Guid>(type: "TEXT", nullable: false, comment: "Идентификатор создателя"),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "Дата создания версии"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "Дата последнего обновления версии"),
                    PublishedAt = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "Дата публикации (если опубликован)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlowVersions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsSystem = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    TelegramUsername = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    TelegramUserId = table.Column<long>(type: "INTEGER", nullable: false),
                    Position = table.Column<string>(type: "TEXT", nullable: true),
                    Language = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastActiveAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FlowSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FlowId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DaysToComplete = table.Column<int>(type: "INTEGER", nullable: true),
                    ExcludeWeekends = table.Column<bool>(type: "INTEGER", nullable: false),
                    ExcludeHolidays = table.Column<bool>(type: "INTEGER", nullable: false),
                    RequiresBuddy = table.Column<bool>(type: "INTEGER", nullable: false),
                    AutoAssignBuddy = table.Column<bool>(type: "INTEGER", nullable: false),
                    AllowSelfPaced = table.Column<bool>(type: "INTEGER", nullable: false),
                    AllowPause = table.Column<bool>(type: "INTEGER", nullable: false),
                    SendDeadlineReminders = table.Column<bool>(type: "INTEGER", nullable: false),
                    FirstReminderDaysBefore = table.Column<int>(type: "INTEGER", nullable: false),
                    FinalReminderDaysBefore = table.Column<int>(type: "INTEGER", nullable: false),
                    SendDailyProgress = table.Column<bool>(type: "INTEGER", nullable: false),
                    SendStepCompletionNotifications = table.Column<bool>(type: "INTEGER", nullable: false),
                    CustomSettings = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlowSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlowSettings_Flows_FlowId",
                        column: x => x.FlowId,
                        principalTable: "Flows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FlowSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FlowId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Order = table.Column<string>(type: "TEXT", nullable: false),
                    IsRequired = table.Column<bool>(type: "INTEGER", nullable: false),
                    EstimatedDurationMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Instructions = table.Column<string>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlowSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlowSteps_Flows_FlowId",
                        column: x => x.FlowId,
                        principalTable: "Flows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FlowStepVersions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OriginalId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "Идентификатор оригинального этапа"),
                    Version = table.Column<int>(type: "INTEGER", nullable: false, comment: "Номер версии"),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false, comment: "Является ли версия активной"),
                    FlowVersionId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "Идентификатор версии потока"),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false, comment: "Название этапа"),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false, comment: "Описание этапа"),
                    Order = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, comment: "Порядок этапа (LexoRank)"),
                    IsRequired = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true, comment: "Является ли этап обязательным"),
                    EstimatedDurationMinutes = table.Column<int>(type: "INTEGER", nullable: false, comment: "Оценочное время выполнения в минутах"),
                    Status = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0, comment: "Статус этапа"),
                    Instructions = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false, defaultValue: "", comment: "Инструкции для этапа"),
                    Notes = table.Column<string>(type: "TEXT", nullable: false, defaultValue: "", comment: "Заметки по этапу"),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "Дата создания версии"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "Дата последнего обновления версии")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlowStepVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlowStepVersions_FlowVersions_FlowVersionId",
                        column: x => x.FlowVersionId,
                        principalTable: "FlowVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FlowAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FlowId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OriginalFlowId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FlowVersionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    BuddyId = table.Column<Guid>(type: "TEXT", nullable: true),
                    AssignedById = table.Column<Guid>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false, defaultValue: "Assigned"),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DueDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastActivityAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PausedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PauseReason = table.Column<string>(type: "TEXT", nullable: true),
                    ProgressPercent = table.Column<int>(type: "INTEGER", nullable: false),
                    CompletedSteps = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalSteps = table.Column<int>(type: "INTEGER", nullable: false),
                    AttemptCount = table.Column<int>(type: "INTEGER", nullable: false),
                    FinalScore = table.Column<int>(type: "INTEGER", nullable: true),
                    AdminNotes = table.Column<string>(type: "TEXT", nullable: false),
                    UserFeedback = table.Column<string>(type: "TEXT", nullable: false),
                    UserRating = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlowAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlowAssignments_FlowVersions_FlowVersionId",
                        column: x => x.FlowVersionId,
                        principalTable: "FlowVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FlowAssignments_Flows_FlowId",
                        column: x => x.FlowId,
                        principalTable: "Flows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FlowAssignments_Users_AssignedById",
                        column: x => x.AssignedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FlowAssignments_Users_BuddyId",
                        column: x => x.BuddyId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_FlowAssignments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Channel = table.Column<int>(type: "INTEGER", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    Metadata = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SentAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ReadAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AttemptCount = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    MaxAttempts = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 3),
                    ErrorMessage = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    RelatedEntityId = table.Column<Guid>(type: "TEXT", nullable: true),
                    RelatedEntityType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserAchievements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AchievementId = table.Column<Guid>(type: "TEXT", nullable: false),
                    EarnedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAchievements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAchievements_Achievements_AchievementId",
                        column: x => x.AchievementId,
                        principalTable: "Achievements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAchievements_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserProgress",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AssignedFlowsCount = table.Column<int>(type: "INTEGER", nullable: false),
                    CompletedFlowsCount = table.Column<int>(type: "INTEGER", nullable: false),
                    ActiveFlowsCount = table.Column<int>(type: "INTEGER", nullable: false),
                    OverdueFlowsCount = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalLearningTimeMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    AchievementsCount = table.Column<int>(type: "INTEGER", nullable: false),
                    LastActivityAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserProgress_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RoleId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Components",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false, defaultValue: "Draft"),
                    EstimatedDurationMinutes = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 15),
                    MaxAttempts = table.Column<int>(type: "INTEGER", nullable: true),
                    MinPassingScore = table.Column<int>(type: "INTEGER", nullable: true),
                    Instructions = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FlowStepId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Order = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    IsRequired = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Components", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Components_FlowSteps_FlowStepId",
                        column: x => x.FlowStepId,
                        principalTable: "FlowSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComponentVersions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OriginalId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "Идентификатор оригинального компонента"),
                    Version = table.Column<int>(type: "INTEGER", nullable: false, comment: "Номер версии"),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false, comment: "Является ли версия активной"),
                    StepVersionId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "Идентификатор версии этапа"),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false, comment: "Название компонента"),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false, comment: "Описание компонента"),
                    ComponentType = table.Column<string>(type: "TEXT", nullable: false, comment: "Тип компонента"),
                    Status = table.Column<string>(type: "TEXT", nullable: false, defaultValue: "Draft", comment: "Статус компонента"),
                    Order = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, comment: "Порядок компонента (LexoRank)"),
                    IsRequired = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true, comment: "Является ли компонент обязательным"),
                    EstimatedDurationMinutes = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 15, comment: "Оценочное время выполнения в минутах"),
                    MaxAttempts = table.Column<int>(type: "INTEGER", nullable: true, comment: "Максимальное количество попыток"),
                    MinPassingScore = table.Column<int>(type: "INTEGER", nullable: true, comment: "Минимальный проходной балл"),
                    Instructions = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false, defaultValue: "", comment: "Инструкции для компонента"),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "Дата создания версии"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "Дата последнего обновления версии")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComponentVersions_FlowStepVersions_StepVersionId",
                        column: x => x.StepVersionId,
                        principalTable: "FlowStepVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FlowProgress",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FlowAssignmentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FlowVersionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CompletedStepsCount = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalStepsCount = table.Column<int>(type: "INTEGER", nullable: false),
                    CompletedComponentsCount = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalComponentsCount = table.Column<int>(type: "INTEGER", nullable: false),
                    TimeSpentMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastUpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CurrentStepId = table.Column<Guid>(type: "TEXT", nullable: true),
                    UserProgressId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlowProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlowProgress_FlowAssignments_FlowAssignmentId",
                        column: x => x.FlowAssignmentId,
                        principalTable: "FlowAssignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FlowProgress_FlowVersions_FlowVersionId",
                        column: x => x.FlowVersionId,
                        principalTable: "FlowVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FlowProgress_UserProgress_UserProgressId",
                        column: x => x.UserProgressId,
                        principalTable: "UserProgress",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FlowProgress_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArticleComponents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    ReadingTimeMinutes = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 15)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleComponents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArticleComponents_Components_Id",
                        column: x => x.Id,
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuizComponents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    QuestionText = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizComponents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizComponents_Components_Id",
                        column: x => x.Id,
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskComponents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Instruction = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    CodeWord = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Hint = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskComponents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskComponents_Components_Id",
                        column: x => x.Id,
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArticleComponentVersions",
                columns: table => new
                {
                    ComponentVersionId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "Идентификатор версии компонента"),
                    Content = table.Column<string>(type: "TEXT", nullable: false, comment: "Содержимое статьи в формате Markdown"),
                    ReadingTimeMinutes = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 15, comment: "Время чтения статьи в минутах")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleComponentVersions", x => x.ComponentVersionId);
                    table.ForeignKey(
                        name: "FK_ArticleComponentVersions_ComponentVersions_ComponentVersionId",
                        column: x => x.ComponentVersionId,
                        principalTable: "ComponentVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuizComponentVersions",
                columns: table => new
                {
                    ComponentVersionId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "Идентификатор версии компонента"),
                    PassingScore = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 80, comment: "Проходной балл (в процентах)"),
                    TimeLimitMinutes = table.Column<int>(type: "INTEGER", nullable: true, comment: "Ограничение по времени в минутах"),
                    AllowMultipleAttempts = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true, comment: "Разрешены ли множественные попытки"),
                    ShowCorrectAnswers = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true, comment: "Показывать ли правильные ответы после завершения"),
                    ShuffleQuestions = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false, comment: "Перемешивать ли вопросы"),
                    ShuffleAnswers = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false, comment: "Перемешивать ли варианты ответов")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizComponentVersions", x => x.ComponentVersionId);
                    table.ForeignKey(
                        name: "FK_QuizComponentVersions_ComponentVersions_ComponentVersionId",
                        column: x => x.ComponentVersionId,
                        principalTable: "ComponentVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskComponentVersions",
                columns: table => new
                {
                    ComponentVersionId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "Идентификатор версии компонента"),
                    Instructions = table.Column<string>(type: "TEXT", nullable: false, comment: "Подробные инструкции по выполнению задания"),
                    SubmissionType = table.Column<string>(type: "TEXT", nullable: false, defaultValue: "Text", comment: "Тип отправки результата"),
                    MaxFileSize = table.Column<int>(type: "INTEGER", nullable: true, comment: "Максимальный размер файла в байтах"),
                    AllowedFileTypes = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true, comment: "Разрешенные типы файлов"),
                    RequiresMentorApproval = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true, comment: "Требуется ли одобрение наставника"),
                    AutoApprovalKeywords = table.Column<string>(type: "TEXT", nullable: true, comment: "Ключевые слова для автоматического одобрения")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskComponentVersions", x => x.ComponentVersionId);
                    table.ForeignKey(
                        name: "FK_TaskComponentVersions_ComponentVersions_ComponentVersionId",
                        column: x => x.ComponentVersionId,
                        principalTable: "ComponentVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StepProgress",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FlowProgressId = table.Column<Guid>(type: "TEXT", nullable: false),
                    StepVersionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    CompletedComponentsCount = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalComponentsCount = table.Column<int>(type: "INTEGER", nullable: false),
                    TimeSpentMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastUpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsUnlocked = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StepProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StepProgress_FlowProgress_FlowProgressId",
                        column: x => x.FlowProgressId,
                        principalTable: "FlowProgress",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StepProgress_FlowStepVersions_StepVersionId",
                        column: x => x.StepVersionId,
                        principalTable: "FlowStepVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionOptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Text = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    IsCorrect = table.Column<bool>(type: "INTEGER", nullable: false),
                    Order = table.Column<string>(type: "TEXT", nullable: false),
                    Message = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Points = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1),
                    QuizComponentId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionOptions_QuizComponents_QuizComponentId",
                        column: x => x.QuizComponentId,
                        principalTable: "QuizComponents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuizOptionVersions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    QuizVersionId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "Идентификатор версии квиза"),
                    Text = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false, comment: "Текст варианта ответа"),
                    IsCorrect = table.Column<bool>(type: "INTEGER", nullable: false, comment: "Является ли ответ правильным"),
                    Points = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0, comment: "Количество баллов за этот ответ"),
                    Order = table.Column<int>(type: "INTEGER", nullable: false, comment: "Порядковый номер варианта ответа"),
                    Explanation = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true, comment: "Объяснение ответа")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizOptionVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizOptionVersions_QuizComponentVersions_QuizVersionId",
                        column: x => x.QuizVersionId,
                        principalTable: "QuizComponentVersions",
                        principalColumn: "ComponentVersionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComponentProgress",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    StepProgressId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ComponentVersionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    IsRequired = table.Column<bool>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    IsCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    AttemptsCount = table.Column<int>(type: "INTEGER", nullable: false),
                    BestScore = table.Column<int>(type: "INTEGER", nullable: true),
                    LastScore = table.Column<int>(type: "INTEGER", nullable: true),
                    TimeSpentMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastUpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComponentProgress_ComponentVersions_ComponentVersionId",
                        column: x => x.ComponentVersionId,
                        principalTable: "ComponentVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComponentProgress_StepProgress_StepProgressId",
                        column: x => x.StepProgressId,
                        principalTable: "StepProgress",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Achievements_CreatedAt",
                table: "Achievements",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Achievements_IsActive",
                table: "Achievements",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Achievements_Rarity",
                table: "Achievements",
                column: "Rarity");

            migrationBuilder.CreateIndex(
                name: "IX_Achievements_Title",
                table: "Achievements",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleComponents_ReadingTimeMinutes",
                table: "ArticleComponents",
                column: "ReadingTimeMinutes");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleComponentVersions_ReadingTime",
                table: "ArticleComponentVersions",
                column: "ReadingTimeMinutes");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentProgress_ComponentVersionId",
                table: "ComponentProgress",
                column: "ComponentVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentProgress_StepProgressId",
                table: "ComponentProgress",
                column: "StepProgressId");

            migrationBuilder.CreateIndex(
                name: "IX_Components_CreatedAt",
                table: "Components",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Components_FlowStepId",
                table: "Components",
                column: "FlowStepId");

            migrationBuilder.CreateIndex(
                name: "IX_Components_Order",
                table: "Components",
                column: "Order");

            migrationBuilder.CreateIndex(
                name: "IX_Components_Status",
                table: "Components",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Components_Title",
                table: "Components",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentVersions_Active",
                table: "ComponentVersions",
                column: "IsActive",
                filter: "IsActive = 1");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentVersions_ComponentType",
                table: "ComponentVersions",
                column: "ComponentType");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentVersions_CreatedAt",
                table: "ComponentVersions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentVersions_Order",
                table: "ComponentVersions",
                column: "Order");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentVersions_OriginalId",
                table: "ComponentVersions",
                column: "OriginalId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentVersions_OriginalId_Active",
                table: "ComponentVersions",
                columns: new[] { "OriginalId", "IsActive" },
                unique: true,
                filter: "IsActive = 1");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentVersions_OriginalId_Version",
                table: "ComponentVersions",
                columns: new[] { "OriginalId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ComponentVersions_Status",
                table: "ComponentVersions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentVersions_StepVersionId",
                table: "ComponentVersions",
                column: "StepVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentVersions_Version",
                table: "ComponentVersions",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_FlowAssignments_AssignedById",
                table: "FlowAssignments",
                column: "AssignedById");

            migrationBuilder.CreateIndex(
                name: "IX_FlowAssignments_BuddyId",
                table: "FlowAssignments",
                column: "BuddyId");

            migrationBuilder.CreateIndex(
                name: "IX_FlowAssignments_CreatedAt",
                table: "FlowAssignments",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_FlowAssignments_DueDate",
                table: "FlowAssignments",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_FlowAssignments_FlowId",
                table: "FlowAssignments",
                column: "FlowId");

            migrationBuilder.CreateIndex(
                name: "IX_FlowAssignments_FlowVersionId",
                table: "FlowAssignments",
                column: "FlowVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_FlowAssignments_Status",
                table: "FlowAssignments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_FlowAssignments_UserId_FlowId",
                table: "FlowAssignments",
                columns: new[] { "UserId", "FlowId" });

            migrationBuilder.CreateIndex(
                name: "IX_FlowProgress_FlowAssignmentId",
                table: "FlowProgress",
                column: "FlowAssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_FlowProgress_FlowVersionId",
                table: "FlowProgress",
                column: "FlowVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_FlowProgress_UserId",
                table: "FlowProgress",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FlowProgress_UserProgressId",
                table: "FlowProgress",
                column: "UserProgressId");

            migrationBuilder.CreateIndex(
                name: "IX_Flows_CreatedAt",
                table: "Flows",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Flows_Status",
                table: "Flows",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Flows_Title",
                table: "Flows",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_FlowSettings_FlowId",
                table: "FlowSettings",
                column: "FlowId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FlowSteps_FlowId",
                table: "FlowSteps",
                column: "FlowId");

            migrationBuilder.CreateIndex(
                name: "IX_FlowStepVersions_Active",
                table: "FlowStepVersions",
                column: "IsActive",
                filter: "IsActive = 1");

            migrationBuilder.CreateIndex(
                name: "IX_FlowStepVersions_CreatedAt",
                table: "FlowStepVersions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_FlowStepVersions_FlowVersionId",
                table: "FlowStepVersions",
                column: "FlowVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_FlowStepVersions_Order",
                table: "FlowStepVersions",
                column: "Order");

            migrationBuilder.CreateIndex(
                name: "IX_FlowStepVersions_OriginalId",
                table: "FlowStepVersions",
                column: "OriginalId");

            migrationBuilder.CreateIndex(
                name: "IX_FlowStepVersions_OriginalId_Active",
                table: "FlowStepVersions",
                columns: new[] { "OriginalId", "IsActive" },
                unique: true,
                filter: "IsActive = 1");

            migrationBuilder.CreateIndex(
                name: "IX_FlowStepVersions_OriginalId_Version",
                table: "FlowStepVersions",
                columns: new[] { "OriginalId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FlowStepVersions_Status",
                table: "FlowStepVersions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_FlowStepVersions_Version",
                table: "FlowStepVersions",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_FlowVersions_Active",
                table: "FlowVersions",
                column: "IsActive",
                filter: "IsActive = 1");

            migrationBuilder.CreateIndex(
                name: "IX_FlowVersions_CreatedAt",
                table: "FlowVersions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_FlowVersions_OriginalId",
                table: "FlowVersions",
                column: "OriginalId");

            migrationBuilder.CreateIndex(
                name: "IX_FlowVersions_OriginalId_Active",
                table: "FlowVersions",
                columns: new[] { "OriginalId", "IsActive" },
                unique: true,
                filter: "IsActive = 1");

            migrationBuilder.CreateIndex(
                name: "IX_FlowVersions_OriginalId_Version",
                table: "FlowVersions",
                columns: new[] { "OriginalId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FlowVersions_Status",
                table: "FlowVersions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_FlowVersions_Version",
                table: "FlowVersions",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CreatedAt",
                table: "Notifications",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Priority",
                table: "Notifications",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_RelatedEntity",
                table: "Notifications",
                columns: new[] { "RelatedEntityType", "RelatedEntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Status_ScheduledAt",
                table: "Notifications",
                columns: new[] { "Status", "ScheduledAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Type",
                table: "Notifications",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionOptions_IsCorrect",
                table: "QuestionOptions",
                column: "IsCorrect");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionOptions_Order",
                table: "QuestionOptions",
                column: "Order");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionOptions_QuizComponentId",
                table: "QuestionOptions",
                column: "QuizComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizComponents_QuestionText",
                table: "QuizComponents",
                column: "QuestionText");

            migrationBuilder.CreateIndex(
                name: "IX_QuizComponentVersions_MultipleAttempts",
                table: "QuizComponentVersions",
                column: "AllowMultipleAttempts");

            migrationBuilder.CreateIndex(
                name: "IX_QuizComponentVersions_PassingScore",
                table: "QuizComponentVersions",
                column: "PassingScore");

            migrationBuilder.CreateIndex(
                name: "IX_QuizComponentVersions_TimeLimit",
                table: "QuizComponentVersions",
                column: "TimeLimitMinutes");

            migrationBuilder.CreateIndex(
                name: "IX_QuizOptionVersions_IsCorrect",
                table: "QuizOptionVersions",
                column: "IsCorrect");

            migrationBuilder.CreateIndex(
                name: "IX_QuizOptionVersions_Points",
                table: "QuizOptionVersions",
                column: "Points");

            migrationBuilder.CreateIndex(
                name: "IX_QuizOptionVersions_QuizVersionId",
                table: "QuizOptionVersions",
                column: "QuizVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizOptionVersions_QuizVersionId_Order",
                table: "QuizOptionVersions",
                columns: new[] { "QuizVersionId", "Order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StepProgress_FlowProgressId",
                table: "StepProgress",
                column: "FlowProgressId");

            migrationBuilder.CreateIndex(
                name: "IX_StepProgress_StepVersionId",
                table: "StepProgress",
                column: "StepVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskComponents_CodeWord",
                table: "TaskComponents",
                column: "CodeWord");

            migrationBuilder.CreateIndex(
                name: "IX_TaskComponentVersions_MaxFileSize",
                table: "TaskComponentVersions",
                column: "MaxFileSize");

            migrationBuilder.CreateIndex(
                name: "IX_TaskComponentVersions_RequiresMentorApproval",
                table: "TaskComponentVersions",
                column: "RequiresMentorApproval");

            migrationBuilder.CreateIndex(
                name: "IX_TaskComponentVersions_SubmissionType",
                table: "TaskComponentVersions",
                column: "SubmissionType");

            migrationBuilder.CreateIndex(
                name: "IX_UserAchievements_AchievementId",
                table: "UserAchievements",
                column: "AchievementId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAchievements_EarnedAt",
                table: "UserAchievements",
                column: "EarnedAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserAchievements_UserId",
                table: "UserAchievements",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAchievements_UserId_AchievementId",
                table: "UserAchievements",
                columns: new[] { "UserId", "AchievementId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserProgress_UserId",
                table: "UserProgress",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedAt",
                table: "Users",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsActive",
                table: "Users",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Users_TelegramUserId",
                table: "Users",
                column: "TelegramUserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleComponents");

            migrationBuilder.DropTable(
                name: "ArticleComponentVersions");

            migrationBuilder.DropTable(
                name: "ComponentProgress");

            migrationBuilder.DropTable(
                name: "FlowSettings");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "QuestionOptions");

            migrationBuilder.DropTable(
                name: "QuizOptionVersions");

            migrationBuilder.DropTable(
                name: "TaskComponents");

            migrationBuilder.DropTable(
                name: "TaskComponentVersions");

            migrationBuilder.DropTable(
                name: "UserAchievements");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "StepProgress");

            migrationBuilder.DropTable(
                name: "QuizComponents");

            migrationBuilder.DropTable(
                name: "QuizComponentVersions");

            migrationBuilder.DropTable(
                name: "Achievements");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "FlowProgress");

            migrationBuilder.DropTable(
                name: "Components");

            migrationBuilder.DropTable(
                name: "ComponentVersions");

            migrationBuilder.DropTable(
                name: "FlowAssignments");

            migrationBuilder.DropTable(
                name: "UserProgress");

            migrationBuilder.DropTable(
                name: "FlowSteps");

            migrationBuilder.DropTable(
                name: "FlowStepVersions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Flows");

            migrationBuilder.DropTable(
                name: "FlowVersions");
        }
    }
}
