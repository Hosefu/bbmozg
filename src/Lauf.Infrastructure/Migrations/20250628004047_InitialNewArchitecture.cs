using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lauf.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialNewArchitecture : Migration
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
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Channel = table.Column<int>(type: "INTEGER", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false, defaultValue: "Pending"),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ScheduledAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SentAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AttemptCount = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxAttempts = table.Column<int>(type: "INTEGER", nullable: false),
                    RelatedEntityId = table.Column<Guid>(type: "TEXT", nullable: true),
                    RelatedEntityType = table.Column<string>(type: "TEXT", nullable: true),
                    UserId1 = table.Column<Guid>(type: "TEXT", nullable: false)
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
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId1",
                        column: x => x.UserId1,
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
                name: "ArticleComponents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleComponents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Components",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Content = table.Column<string>(type: "TEXT", maxLength: 10000, nullable: false),
                    FlowStepId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Order = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    IsRequired = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    IsEnabled = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Components", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuizComponents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AllowMultipleAttempts = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShuffleQuestions = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShuffleOptions = table.Column<bool>(type: "INTEGER", nullable: false)
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
                    CodeWord = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Score = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1),
                    IsCaseSensitive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false)
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
                name: "QuizQuestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    QuizComponentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Text = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    IsRequired = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    Order = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizQuestions_QuizComponents_QuizComponentId",
                        column: x => x.QuizComponentId,
                        principalTable: "QuizComponents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionOptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    QuizQuestionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Text = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    IsCorrect = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    Score = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1),
                    Order = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionOptions_QuizQuestions_QuizQuestionId",
                        column: x => x.QuizQuestionId,
                        principalTable: "QuizQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FlowAssignmentBuddies",
                columns: table => new
                {
                    BuddiesId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FlowAssignmentId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlowAssignmentBuddies", x => new { x.BuddiesId, x.FlowAssignmentId });
                    table.ForeignKey(
                        name: "FK_FlowAssignmentBuddies_Users_BuddiesId",
                        column: x => x.BuddiesId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FlowAssignmentProgress",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FlowAssignmentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProgressPercent = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    CompletedSteps = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    TotalSteps = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    AttemptCount = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1),
                    FinalScore = table.Column<int>(type: "INTEGER", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastActivityAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PausedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PauseReason = table.Column<string>(type: "TEXT", nullable: true),
                    UserRating = table.Column<int>(type: "INTEGER", nullable: true),
                    UserFeedback = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    FlowAssignmentId1 = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlowAssignmentProgress", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FlowAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FlowId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FlowContentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AssignedBy = table.Column<Guid>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false, defaultValue: "Assigned"),
                    AssignedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FlowContentId1 = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlowAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlowAssignments_Users_AssignedBy",
                        column: x => x.AssignedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FlowAssignments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FlowContents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FlowId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Version = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: false),
                    FlowId1 = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlowContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlowContents_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Flows",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    ActiveContentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Flows_FlowContents_ActiveContentId",
                        column: x => x.ActiveContentId,
                        principalTable: "FlowContents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Flows_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FlowSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FlowContentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Order = table.Column<string>(type: "TEXT", nullable: false),
                    IsEnabled = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlowSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlowSteps_FlowContents_FlowContentId",
                        column: x => x.FlowContentId,
                        principalTable: "FlowContents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FlowSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FlowId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DaysPerStep = table.Column<int>(type: "INTEGER", nullable: false),
                    RequireSequentialCompletionComponents = table.Column<bool>(type: "INTEGER", nullable: false),
                    AllowSelfRestart = table.Column<bool>(type: "INTEGER", nullable: false),
                    AllowSelfPause = table.Column<bool>(type: "INTEGER", nullable: false),
                    SendStartNotification = table.Column<bool>(type: "INTEGER", nullable: false),
                    SendProgressReminders = table.Column<bool>(type: "INTEGER", nullable: false),
                    SendCompletionNotification = table.Column<bool>(type: "INTEGER", nullable: false),
                    ReminderInterval = table.Column<TimeSpan>(type: "TEXT", nullable: false),
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
                name: "IX_Components_FlowStepId",
                table: "Components",
                column: "FlowStepId");

            migrationBuilder.CreateIndex(
                name: "IX_FlowAssignmentBuddies_FlowAssignmentId",
                table: "FlowAssignmentBuddies",
                column: "FlowAssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_FlowAssignmentProgress_FlowAssignmentId",
                table: "FlowAssignmentProgress",
                column: "FlowAssignmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FlowAssignmentProgress_FlowAssignmentId1",
                table: "FlowAssignmentProgress",
                column: "FlowAssignmentId1");

            migrationBuilder.CreateIndex(
                name: "IX_FlowAssignments_AssignedAt",
                table: "FlowAssignments",
                column: "AssignedAt");

            migrationBuilder.CreateIndex(
                name: "IX_FlowAssignments_AssignedBy",
                table: "FlowAssignments",
                column: "AssignedBy");

            migrationBuilder.CreateIndex(
                name: "IX_FlowAssignments_FlowContentId",
                table: "FlowAssignments",
                column: "FlowContentId");

            migrationBuilder.CreateIndex(
                name: "IX_FlowAssignments_FlowContentId1",
                table: "FlowAssignments",
                column: "FlowContentId1");

            migrationBuilder.CreateIndex(
                name: "IX_FlowAssignments_FlowId",
                table: "FlowAssignments",
                column: "FlowId");

            migrationBuilder.CreateIndex(
                name: "IX_FlowAssignments_Status",
                table: "FlowAssignments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_FlowAssignments_UserId_FlowId",
                table: "FlowAssignments",
                columns: new[] { "UserId", "FlowId" });

            migrationBuilder.CreateIndex(
                name: "IX_FlowContents_CreatedByUserId",
                table: "FlowContents",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FlowContents_FlowId",
                table: "FlowContents",
                column: "FlowId");

            migrationBuilder.CreateIndex(
                name: "IX_FlowContents_FlowId_Version",
                table: "FlowContents",
                columns: new[] { "FlowId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FlowContents_FlowId1",
                table: "FlowContents",
                column: "FlowId1");

            migrationBuilder.CreateIndex(
                name: "IX_Flows_ActiveContentId",
                table: "Flows",
                column: "ActiveContentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Flows_CreatedAt",
                table: "Flows",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Flows_CreatedBy",
                table: "Flows",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Flows_CreatedByUserId",
                table: "Flows",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Flows_IsActive",
                table: "Flows",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Flows_Name",
                table: "Flows",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_FlowSettings_FlowId",
                table: "FlowSettings",
                column: "FlowId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FlowSteps_FlowContentId",
                table: "FlowSteps",
                column: "FlowContentId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId1",
                table: "Notifications",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionOptions_QuizQuestionId",
                table: "QuestionOptions",
                column: "QuizQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestions_Order",
                table: "QuizQuestions",
                column: "Order");

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestions_QuizComponentId",
                table: "QuizQuestions",
                column: "QuizComponentId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleComponents_Components_Id",
                table: "ArticleComponents",
                column: "Id",
                principalTable: "Components",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Components_FlowSteps_FlowStepId",
                table: "Components",
                column: "FlowStepId",
                principalTable: "FlowSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FlowAssignmentBuddies_FlowAssignments_FlowAssignmentId",
                table: "FlowAssignmentBuddies",
                column: "FlowAssignmentId",
                principalTable: "FlowAssignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FlowAssignmentProgress_FlowAssignments_FlowAssignmentId",
                table: "FlowAssignmentProgress",
                column: "FlowAssignmentId",
                principalTable: "FlowAssignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FlowAssignmentProgress_FlowAssignments_FlowAssignmentId1",
                table: "FlowAssignmentProgress",
                column: "FlowAssignmentId1",
                principalTable: "FlowAssignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FlowAssignments_FlowContents_FlowContentId",
                table: "FlowAssignments",
                column: "FlowContentId",
                principalTable: "FlowContents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FlowAssignments_FlowContents_FlowContentId1",
                table: "FlowAssignments",
                column: "FlowContentId1",
                principalTable: "FlowContents",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FlowAssignments_Flows_FlowId",
                table: "FlowAssignments",
                column: "FlowId",
                principalTable: "Flows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FlowContents_Flows_FlowId",
                table: "FlowContents",
                column: "FlowId",
                principalTable: "Flows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FlowContents_Flows_FlowId1",
                table: "FlowContents",
                column: "FlowId1",
                principalTable: "Flows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlowContents_Users_CreatedByUserId",
                table: "FlowContents");

            migrationBuilder.DropForeignKey(
                name: "FK_Flows_Users_CreatedByUserId",
                table: "Flows");

            migrationBuilder.DropForeignKey(
                name: "FK_Flows_FlowContents_ActiveContentId",
                table: "Flows");

            migrationBuilder.DropTable(
                name: "ArticleComponents");

            migrationBuilder.DropTable(
                name: "FlowAssignmentBuddies");

            migrationBuilder.DropTable(
                name: "FlowAssignmentProgress");

            migrationBuilder.DropTable(
                name: "FlowSettings");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "QuestionOptions");

            migrationBuilder.DropTable(
                name: "TaskComponents");

            migrationBuilder.DropTable(
                name: "UserAchievements");

            migrationBuilder.DropTable(
                name: "UserProgress");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "FlowAssignments");

            migrationBuilder.DropTable(
                name: "QuizQuestions");

            migrationBuilder.DropTable(
                name: "Achievements");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "QuizComponents");

            migrationBuilder.DropTable(
                name: "Components");

            migrationBuilder.DropTable(
                name: "FlowSteps");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "FlowContents");

            migrationBuilder.DropTable(
                name: "Flows");
        }
    }
}
