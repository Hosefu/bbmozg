using Microsoft.EntityFrameworkCore;
using Lauf.Domain.Entities.Flows;
using Lauf.Domain.Entities.Components;
using Lauf.Domain.Entities.Users;

namespace Lauf.Infrastructure.Persistence.Seeds;

/// <summary>
/// Seeder для создания демонстрационного потока "Дизайн-студия: Основы работы"
/// </summary>
public static class DesignStudioFlowSeeder
{
    /// <summary>
    /// Создает полноценный поток обучения для дизайн-студии
    /// </summary>
    /// <param name="context">Контекст базы данных</param>
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Проверяем, есть ли уже этот поток в базе
        if (await context.Flows.AnyAsync(f => f.Name == "Дизайн-студия: Основы работы"))
        {
            return; // Данные уже есть
        }

        // Получаем системного пользователя (создаем если нет)
        var systemUser = await GetOrCreateSystemUser(context);

        // Создаем поток без ActiveContentId
        var flow = new Flow(
            "Дизайн-студия: Основы работы",
            "Комплексное введение в работу дизайн-студии: корпоративные ценности, техническое оснащение и управление файлами",
            systemUser.Id
        );
        
        // Сбрасываем ActiveContentId в null для начала
        flow.SetActiveContent(null);

        // Создаем настройки потока
        var settings = new FlowSettings
        {
            Id = Guid.NewGuid(),
            FlowId = flow.Id,
            DaysPerStep = 2,
            RequireSequentialCompletionComponents = true,
            SendStartNotification = true,
            SendProgressReminders = true,
            SendCompletionNotification = true
        };

        // Добавляем flow и settings сначала без активного контента
        await context.Flows.AddAsync(flow);
        await context.FlowSettings.AddAsync(settings);

        // Сохраняем, чтобы получить ID потока
        await context.SaveChangesAsync();

        // Создаем версию контента
        var content = flow.CreateNewContentVersion(systemUser.Id);
        await context.FlowContents.AddAsync(content);

        // Сохраняем контент
        await context.SaveChangesAsync();

        // Теперь устанавливаем активную версию
        flow.SetActiveContent(content.Id);

        // Финальное сохранение
        await context.SaveChangesAsync();

        // Создаем этапы с компонентами
        await CreateStep1_Values(context, content.Id);
        await CreateStep2_TechnicalEquipment(context, content.Id);
        await CreateStep3_FileManagement(context, content.Id);

        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Получает или создает системного пользователя
    /// </summary>
    private static async Task<User> GetOrCreateSystemUser(ApplicationDbContext context)
    {
        var systemUser = await context.Users.FirstOrDefaultAsync(u => u.FirstName == "System" && u.LastName == "Administrator");
        
        if (systemUser != null)
        {
            return systemUser;
        }

        systemUser = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "System",
            LastName = "Administrator",
            TelegramUsername = "system_admin",
            TelegramUserId = new Domain.ValueObjects.TelegramUserId(1),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        await context.Users.AddAsync(systemUser);
        await context.SaveChangesAsync();

        return systemUser;
    }

    /// <summary>
    /// Этап 1: Ценности дизайн-студии
    /// </summary>
    private static async Task CreateStep1_Values(ApplicationDbContext context, Guid contentId)
    {
        var step = new FlowStep(
            contentId,
            "1. Ценности и философия дизайн-студии",
            "Изучение корпоративных ценностей, принципов работы и культуры команды",
            "1000"
        );

        await context.FlowSteps.AddAsync(step);
        await context.SaveChangesAsync();

        // Статья о ценностях
        var valuesArticle = new ArticleComponent(
            step.Id,
            "Наши ценности",
            "Корпоративные ценности и принципы работы дизайн-студии",
            @"# Ценности нашей дизайн-студии

## Креативность и инновации
Мы стремимся создавать уникальные и инновационные решения, которые выделяют наших клиентов на рынке.

## Качество превыше всего
Каждый проект - это отражение нашего профессионализма. Мы не идем на компромиссы в вопросах качества.

## Командная работа
Лучшие результаты достигаются только в команде. Мы поддерживаем друг друга и делимся знаниями.

## Клиентоориентированность
Успех клиента - это наш успех. Мы всегда ставим интересы клиента на первое место.

## Постоянное развитие
Мир дизайна быстро меняется, и мы должны идти в ногу со временем, постоянно учиться и совершенствоваться.",
            "1000"
        );

        // Тест на понимание ценностей
        var valuesQuiz = new QuizComponent(
            step.Id,
            "Проверка знаний о ценностях",
            "Тест на понимание корпоративных ценностей студии",
            "Ответьте на вопросы о ценностях нашей студии",
            "2000"
        );

        // Задание на рефлексию
        var reflectionTask = new TaskComponent(
            step.Id,
            "Рефлексия по ценностям",
            "Подумайте, какая из наших ценностей вам ближе всего и почему. Найдите кодовое слово в статье о ценностях.",
            "После прочтения статьи о ценностях, найдите ключевое слово, которое объединяет все наши принципы работы.",
            "КОМАНДА",
            5,
            "3000"
        );

        await context.Components.AddRangeAsync(valuesArticle, valuesQuiz, reflectionTask);
        await context.SaveChangesAsync();

        // Добавляем вопросы к тесту
        await AddValuesQuizQuestions(context, valuesQuiz.Id);
    }

    /// <summary>
    /// Этап 2: Техническое обеспечение
    /// </summary>
    private static async Task CreateStep2_TechnicalEquipment(ApplicationDbContext context, Guid contentId)
    {
        var step = new FlowStep(
            contentId,
            "2. Техническое оснащение студии",
            "Изучение программного обеспечения, оборудования и технических стандартов",
            "2000"
        );

        await context.FlowSteps.AddAsync(step);
        await context.SaveChangesAsync();

        // Статья о ПО
        var softwareArticle = new ArticleComponent(
            step.Id,
            "Программное обеспечение",
            "Обзор основного ПО, используемого в студии",
            @"# Программное обеспечение студии

## Adobe Creative Suite
Основной инструментарий дизайнера:
- **Photoshop** - обработка растровых изображений
- **Illustrator** - векторная графика и иллюстрации  
- **InDesign** - верстка и макетирование
- **After Effects** - анимация и видеомонтаж

## Дополнительные инструменты
- **Figma** - UI/UX дизайн и прототипирование
- **Sketch** - дизайн интерфейсов (macOS)
- **Canva** - быстрое создание простых макетов
- **Adobe XD** - дизайн пользовательского опыта

## Организация работы
- **Trello/Asana** - управление проектами
- **Slack** - корпоративная коммуникация
- **Zoom** - видеоконференции с клиентами",
            "1000"
        );

        // Статья об оборудовании
        var hardwareArticle = new ArticleComponent(
            step.Id,
            "Оборудование студии",
            "Технические требования и доступное оборудование",
            @"# Техническое оборудование

## Рабочие станции
- **iMac 27''** - основные рабочие места дизайнеров
- **MacBook Pro 16''** - мобильные рабочие станции
- **Графические планшеты Wacom** - для цифрового рисования

## Периферия
- **Мониторы с калибровкой цвета** - точная цветопередача
- **Принтеры большого формата** - печать макетов и проб
- **Сканеры высокого разрешения** - оцифровка материалов

## Сетевая инфраструктура
- Высокскоростной интернет (1 Гбит/с)
- Сетевое хранилище (NAS) для совместной работы
- Резервное копирование данных",
            "2000"
        );

        // Тест по техническому оснащению
        var techQuiz = new QuizComponent(
            step.Id,
            "Знание технического оснащения",
            "Проверка знаний о программах и оборудовании студии",
            "Ответьте на вопросы о техническом оснащении",
            "3000"
        );

        // Практическое задание
        var practicalTask = new TaskComponent(
            step.Id,
            "Практическое задание",
            "Создайте простой макет визитки в любой доступной программе и найдите кодовое слово",
            "Используя любую из изученных программ, создайте макет визитки размером 90x50мм. Кодовое слово скрыто в разделе о сетевой инфраструктуре.",
            "NAS",
            10,
            "4000"
        );

        await context.Components.AddRangeAsync(softwareArticle, hardwareArticle, techQuiz, practicalTask);
        await context.SaveChangesAsync();

        // Добавляем вопросы к тесту
        await AddTechQuizQuestions(context, techQuiz.Id);
    }

    /// <summary>
    /// Этап 3: Управление файлами
    /// </summary>
    private static async Task CreateStep3_FileManagement(ApplicationDbContext context, Guid contentId)
    {
        var step = new FlowStep(
            contentId,
            "3. Организация и хранение файлов",
            "Изучение принципов организации файлов, системы именования и резервного копирования",
            "3000"
        );

        await context.FlowSteps.AddAsync(step);
        await context.SaveChangesAsync();

        // Статья о структуре файлов
        var fileStructureArticle = new ArticleComponent(
            step.Id,
            "Структура папок и файлов",
            "Стандарты организации файловой системы в проектах",
            @"# Организация файлов в проектах

## Структура папок проекта
```
Проект_НазваниеКлиента_ГГММ/
├── 01_Техническое_задание/
│   ├── ТЗ_от_клиента.pdf
│   └── Брифинг_заметки.txt
├── 02_Исследование/
│   ├── Референсы/
│   └── Анализ_конкурентов/
├── 03_Концепция/
│   ├── Скетчи/
│   └── Мудборд/
├── 04_Дизайн/
│   ├── Исходники/
│   ├── Экспорт/
│   └── Презентация/
└── 05_Финальные_файлы/
    ├── Для_печати/
    └── Для_веб/
```

## Правила именования файлов
- Используйте дату в формате ГГММДД
- Указывайте версию файла (v01, v02, etc.)
- Не используйте пробелы и спецсимволы
- Пример: `Logo_Компания_220315_v03.ai`",
            "1000"
        );

        // Статья о резервном копировании
        var backupArticle = new ArticleComponent(
            step.Id,
            "Резервное копирование",
            "Принципы сохранения и архивирования проектных файлов",
            @"# Система резервного копирования

## Правило 3-2-1
- **3 копии** важных данных
- **2 разных носителя** (локальный диск + облако)
- **1 копия вне офиса** (облачное хранилище)

## Инструменты резервного копирования
- **Time Machine** (macOS) - автоматическое резервное копирование
- **Google Drive/Dropbox** - облачная синхронизация
- **Adobe Creative Cloud** - хранение проектов Adobe

## График архивирования
- **Ежедневно** - автосохранение активных проектов
- **Еженедельно** - полное резервное копирование
- **Ежемесячно** - архивирование завершенных проектов

**Секретное слово для задания: АРХИВ**",
            "2000"
        );

        // Итоговый тест
        var finalQuiz = new QuizComponent(
            step.Id,
            "Итоговый тест по управлению файлами",
            "Комплексная проверка знаний об организации файлов",
            "Проверьте свои знания о правилах работы с файлами",
            "3000"
        );

        // Финальное задание
        var finalTask = new TaskComponent(
            step.Id,
            "Создание файловой структуры",
            "Продемонстрируйте знание принципов организации файлов",
            "Создайте на своем компьютере папку с правильной структурой для нового проекта. Кодовое слово найдите в статье о резервном копировании.",
            "АРХИВ",
            15,
            "4000"
        );

        await context.Components.AddRangeAsync(fileStructureArticle, backupArticle, finalQuiz, finalTask);
        await context.SaveChangesAsync();

        // Добавляем вопросы к тесту
        await AddFileManagementQuizQuestions(context, finalQuiz.Id);
    }

    /// <summary>
    /// Добавляет вопросы к тесту о ценностях
    /// </summary>
    private static async Task AddValuesQuizQuestions(ApplicationDbContext context, Guid quizId)
    {
        var question1 = new QuizQuestion(quizId, "Какая ценность является основополагающей для работы в команде?", "1000");
        var question2 = new QuizQuestion(quizId, "Что мы ставим превыше всего в наших проектах?", "2000");

        await context.QuizQuestions.AddRangeAsync(question1, question2);
        await context.SaveChangesAsync();

        // Варианты для первого вопроса
        var q1Options = new[]
        {
            new QuestionOption(question1.Id, "Скорость выполнения", false, 0, "1"),
            new QuestionOption(question1.Id, "Командная работа", true, 5, "2"),
            new QuestionOption(question1.Id, "Индивидуальные достижения", false, 0, "3"),
            new QuestionOption(question1.Id, "Конкуренция", false, 0, "4")
        };

        // Варианты для второго вопроса
        var q2Options = new[]
        {
            new QuestionOption(question2.Id, "Прибыль", false, 0, "1"),
            new QuestionOption(question2.Id, "Скорость", false, 0, "2"),
            new QuestionOption(question2.Id, "Качество", true, 5, "3"),
            new QuestionOption(question2.Id, "Количество проектов", false, 0, "4")
        };

        await context.QuestionOptions.AddRangeAsync(q1Options.Concat(q2Options));
    }

    /// <summary>
    /// Добавляет вопросы к тесту о техническом оснащении
    /// </summary>
    private static async Task AddTechQuizQuestions(ApplicationDbContext context, Guid quizId)
    {
        var question1 = new QuizQuestion(quizId, "Какая программа используется для векторной графики?", "1000");
        var question2 = new QuizQuestion(quizId, "Что используется для UI/UX дизайна?", "2000");
        var question3 = new QuizQuestion(quizId, "Какое оборудование нужно для точной цветопередачи?", "3000");

        await context.QuizQuestions.AddRangeAsync(question1, question2, question3);
        await context.SaveChangesAsync();

        var q1Options = new[]
        {
            new QuestionOption(question1.Id, "Photoshop", false, 0, "1"),
            new QuestionOption(question1.Id, "Illustrator", true, 3, "2"),
            new QuestionOption(question1.Id, "InDesign", false, 0, "3")
        };

        var q2Options = new[]
        {
            new QuestionOption(question2.Id, "Photoshop", false, 0, "1"),
            new QuestionOption(question2.Id, "Figma", true, 3, "2"),
            new QuestionOption(question2.Id, "After Effects", false, 0, "3")
        };

        var q3Options = new[]
        {
            new QuestionOption(question3.Id, "Обычные мониторы", false, 0, "1"),
            new QuestionOption(question3.Id, "Мониторы с калибровкой цвета", true, 4, "2"),
            new QuestionOption(question3.Id, "Телевизор", false, 0, "3")
        };

        await context.QuestionOptions.AddRangeAsync(q1Options.Concat(q2Options).Concat(q3Options));
    }

    /// <summary>
    /// Добавляет вопросы к тесту по управлению файлами
    /// </summary>
    private static async Task AddFileManagementQuizQuestions(ApplicationDbContext context, Guid quizId)
    {
        var question1 = new QuizQuestion(quizId, "Сколько копий файлов рекомендует правило 3-2-1?", "1000");
        var question2 = new QuizQuestion(quizId, "В каком формате указывается дата в имени файла?", "2000");
        var question3 = new QuizQuestion(quizId, "Как часто нужно делать полное резервное копирование?", "3000");

        await context.QuizQuestions.AddRangeAsync(question1, question2, question3);
        await context.SaveChangesAsync();

        var q1Options = new[]
        {
            new QuestionOption(question1.Id, "2 копии", false, 0, "1"),
            new QuestionOption(question1.Id, "3 копии", true, 3, "2"),
            new QuestionOption(question1.Id, "5 копий", false, 0, "3")
        };

        var q2Options = new[]
        {
            new QuestionOption(question2.Id, "ДД.ММ.ГГ", false, 0, "1"),
            new QuestionOption(question2.Id, "ГГММДД", true, 3, "2"),
            new QuestionOption(question2.Id, "ММ/ДД/ГГГГ", false, 0, "3")
        };

        var q3Options = new[]
        {
            new QuestionOption(question3.Id, "Ежедневно", false, 0, "1"),
            new QuestionOption(question3.Id, "Еженедельно", true, 4, "2"),
            new QuestionOption(question3.Id, "Ежемесячно", false, 0, "3")
        };

        await context.QuestionOptions.AddRangeAsync(q1Options.Concat(q2Options).Concat(q3Options));
    }
}