using HotChocolate.Types;
using Lauf.Application.DTOs.Components;

namespace Lauf.Api.GraphQL.Types.Components;

/// <summary>
/// Union тип для всех типов компонентов
/// </summary>
public class ComponentUnionType : UnionType
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
public class ArticleComponentType : ObjectType<ArticleComponentDto>
{
    protected override void Configure(IObjectTypeDescriptor<ArticleComponentDto> descriptor)
    {
        descriptor.Name("ArticleComponent");
        descriptor.Description("Компонент статьи");

        descriptor.Field(x => x.Id)
            .Description("Уникальный идентификатор");

        descriptor.Field(x => x.Title)
            .Description("Название статьи");

        descriptor.Field(x => x.Description)
            .Description("Описание статьи");

        descriptor.Field(x => x.Content)
            .Description("Содержимое статьи в формате Markdown");

        descriptor.Field(x => x.Type)
            .Description("Тип компонента");

        descriptor.Field(x => x.ReadingTimeMinutes)
            .Description("Примерное время чтения в минутах");
    }
}

/// <summary>
/// GraphQL тип для QuizComponent
/// </summary>
public class QuizComponentType : ObjectType<QuizComponentDto>
{
    protected override void Configure(IObjectTypeDescriptor<QuizComponentDto> descriptor)
    {
        descriptor.Name("QuizComponent");
        descriptor.Description("Компонент квиза");

        descriptor.Field(x => x.Id)
            .Description("Уникальный идентификатор");

        descriptor.Field(x => x.Title)
            .Description("Название квиза");

        descriptor.Field(x => x.Description)
            .Description("Описание квиза");

        descriptor.Field(x => x.Content)
            .Description("Содержимое квиза");

        descriptor.Field(x => x.Type)
            .Description("Тип компонента");

        descriptor.Field(x => x.Questions)
            .Description("Вопросы квиза")
            .Type<ListType<QuizQuestionType>>();
    }
}

/// <summary>
/// GraphQL тип для TaskComponent
/// </summary>
public class TaskComponentType : ObjectType<TaskComponentDto>
{
    protected override void Configure(IObjectTypeDescriptor<TaskComponentDto> descriptor)
    {
        descriptor.Name("TaskComponent");
        descriptor.Description("Компонент задания");

        descriptor.Field(x => x.Id)
            .Description("Уникальный идентификатор");

        descriptor.Field(x => x.Title)
            .Description("Название задания");

        descriptor.Field(x => x.Description)
            .Description("Описание задания");

        descriptor.Field(x => x.Content)
            .Description("Содержимое задания");

        descriptor.Field(x => x.Type)
            .Description("Тип компонента");

        descriptor.Field(x => x.Score)
            .Description("Количество очков за правильное выполнение");
    }
}

/// <summary>
/// GraphQL тип для QuizQuestion
/// </summary>
public class QuizQuestionType : ObjectType<QuizQuestionDto>
{
    protected override void Configure(IObjectTypeDescriptor<QuizQuestionDto> descriptor)
    {
        descriptor.Name("QuizQuestion");
        descriptor.Description("Вопрос квиза");

        descriptor.Field(x => x.Id)
            .Description("Уникальный идентификатор вопроса");

        descriptor.Field(x => x.Text)
            .Description("Текст вопроса");

        descriptor.Field(x => x.IsRequired)
            .Description("Является ли вопрос обязательным");

        descriptor.Field(x => x.Order)
            .Description("Порядковый номер вопроса");

        descriptor.Field(x => x.Options)
            .Description("Варианты ответов")
            .Type<ListType<QuestionOptionType>>();
    }
}

/// <summary>
/// GraphQL тип для QuestionOption
/// </summary>
public class QuestionOptionType : ObjectType<QuestionOptionDto>
{
    protected override void Configure(IObjectTypeDescriptor<QuestionOptionDto> descriptor)
    {
        descriptor.Name("QuestionOption");
        descriptor.Description("Вариант ответа на вопрос");

        descriptor.Field(x => x.Id)
            .Description("Уникальный идентификатор варианта");

        descriptor.Field(x => x.Text)
            .Description("Текст варианта ответа");

        descriptor.Field(x => x.IsCorrect)
            .Description("Является ли этот вариант правильным ответом");

        descriptor.Field(x => x.Order)
            .Description("Порядковый номер варианта");

        descriptor.Field(x => x.Score)
            .Description("Очки за правильный ответ");
    }
}