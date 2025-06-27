using HotChocolate.Types;
using Lauf.Domain.Entities.Components;

namespace Lauf.Api.GraphQL.Types.Components;

/// <summary>
/// Union тип для всех типов компонентов
/// </summary>
public class ComponentUnionType : UnionType<ComponentBase>
{
    protected override void Configure(IUnionTypeDescriptor descriptor)
    {
        descriptor.Name("Component");
        descriptor.Description("Компонент контента (статья, квиз или задание)");
        
        descriptor.Type<ArticleComponentType>();
        descriptor.Type<QuizComponentType>();
        descriptor.Type<TaskComponentType>();
    }
}

/// <summary>
/// GraphQL тип для ArticleComponent
/// </summary>
public class ArticleComponentType : ObjectType<ArticleComponent>
{
    protected override void Configure(IObjectTypeDescriptor<ArticleComponent> descriptor)
    {
        descriptor.Name("ArticleComponent");
        descriptor.Description("Компонент статьи");

        descriptor.Field(x => x.Id)
            .Description("Уникальный идентификатор компонента");

        descriptor.Field(x => x.Type)
            .Description("Тип компонента");

        descriptor.Field(x => x.Title)
            .Description("Название статьи");

        descriptor.Field(x => x.Description)
            .Description("Описание статьи");

        descriptor.Field(x => x.Content)
            .Description("Содержимое статьи в формате Markdown");

        descriptor.Field(x => x.ReadingTimeMinutes)
            .Description("Время чтения в минутах");

        descriptor.Field(x => x.Status)
            .Description("Статус компонента");

        descriptor.Field(x => x.EstimatedDurationMinutes)
            .Description("Приблизительное время выполнения в минутах");

        descriptor.Field(x => x.CreatedAt)
            .Description("Дата создания");

        descriptor.Field(x => x.UpdatedAt)
            .Description("Дата последнего обновления");
    }
}

/// <summary>
/// GraphQL тип для QuizComponent
/// </summary>
public class QuizComponentType : ObjectType<QuizComponent>
{
    protected override void Configure(IObjectTypeDescriptor<QuizComponent> descriptor)
    {
        descriptor.Name("QuizComponent");
        descriptor.Description("Компонент квиза");

        descriptor.Field(x => x.Id)
            .Description("Уникальный идентификатор компонента");

        descriptor.Field(x => x.Type)
            .Description("Тип компонента");

        descriptor.Field(x => x.Title)
            .Description("Название квиза");

        descriptor.Field(x => x.Description)
            .Description("Описание квиза");

        descriptor.Field(x => x.QuestionText)
            .Description("Текст вопроса");

        descriptor.Field(x => x.Options)
            .Description("Варианты ответов");

        descriptor.Field(x => x.Status)
            .Description("Статус компонента");

        descriptor.Field(x => x.EstimatedDurationMinutes)
            .Description("Приблизительное время выполнения в минутах");

        descriptor.Field(x => x.CreatedAt)
            .Description("Дата создания");

        descriptor.Field(x => x.UpdatedAt)
            .Description("Дата последнего обновления");
    }
}

/// <summary>
/// GraphQL тип для TaskComponent
/// </summary>
public class TaskComponentType : ObjectType<TaskComponent>
{
    protected override void Configure(IObjectTypeDescriptor<TaskComponent> descriptor)
    {
        descriptor.Name("TaskComponent");
        descriptor.Description("Компонент задания");

        descriptor.Field(x => x.Id)
            .Description("Уникальный идентификатор компонента");

        descriptor.Field(x => x.Type)
            .Description("Тип компонента");

        descriptor.Field(x => x.Title)
            .Description("Название задания");

        descriptor.Field(x => x.Description)
            .Description("Описание задания");

        descriptor.Field(x => x.Instruction)
            .Description("Инструкция для выполнения задания");

        descriptor.Field(x => x.CodeWord)
            .Description("Кодовое слово для проверки");

        descriptor.Field(x => x.Hint)
            .Description("Подсказка для задания");

        descriptor.Field(x => x.Status)
            .Description("Статус компонента");

        descriptor.Field(x => x.EstimatedDurationMinutes)
            .Description("Приблизительное время выполнения в минутах");

        descriptor.Field(x => x.CreatedAt)
            .Description("Дата создания");

        descriptor.Field(x => x.UpdatedAt)
            .Description("Дата последнего обновления");
    }
}

/// <summary>
/// GraphQL тип для QuestionOption
/// </summary>
public class QuestionOptionType : ObjectType<QuestionOption>
{
    protected override void Configure(IObjectTypeDescriptor<QuestionOption> descriptor)
    {
        descriptor.Name("QuestionOption");
        descriptor.Description("Вариант ответа для квиза");

        descriptor.Field(x => x.Id)
            .Description("Уникальный идентификатор варианта");

        descriptor.Field(x => x.Text)
            .Description("Текст варианта ответа");

        descriptor.Field(x => x.IsCorrect)
            .Description("Является ли вариант правильным");

        descriptor.Field(x => x.Message)
            .Description("Сообщение при выборе варианта");

        descriptor.Field(x => x.Points)
            .Description("Количество баллов за вариант");

        descriptor.Field(x => x.Order)
            .Description("Порядковый номер варианта");
    }
}