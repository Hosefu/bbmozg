using Lauf.Application.DTOs.Users;

namespace Lauf.Api.GraphQL.Types;

/// <summary>
/// GraphQL тип для пользователя
/// </summary>
public class UserType : ObjectType<UserDto>
{
    protected override void Configure(IObjectTypeDescriptor<UserDto> descriptor)
    {
        descriptor.Name("User");
        descriptor.Description("Пользователь системы");

        descriptor.Field(f => f.Id)
            .Description("Уникальный идентификатор пользователя");

        descriptor.Field(f => f.TelegramUserId)
            .Description("Идентификатор пользователя в Telegram");

        descriptor.Field(f => f.FirstName)
            .Description("Имя пользователя");

        descriptor.Field(f => f.LastName)
            .Description("Фамилия пользователя");

        descriptor.Field(f => f.Username)
            .Description("Username в Telegram");

        descriptor.Field(f => f.Position)
            .Description("Должность пользователя");

        descriptor.Field(f => f.Department)
            .Description("Департамент пользователя");

        descriptor.Field(f => f.IsActive)
            .Description("Активен ли пользователь");

        descriptor.Field(f => f.Language)
            .Description("Язык интерфейса пользователя");

        descriptor.Field(f => f.Timezone)
            .Description("Часовой пояс пользователя");

        descriptor.Field(f => f.Roles)
            .Description("Роли пользователя");

        descriptor.Field(f => f.CreatedAt)
            .Description("Дата создания пользователя");

        descriptor.Field(f => f.LastActivityAt)
            .Description("Дата последней активности пользователя");

        descriptor.Field(f => f.DisplayName)
            .Description("Отображаемое имя пользователя");

        descriptor.Field(f => f.FullName)
            .Description("Полное имя пользователя");
    }
}