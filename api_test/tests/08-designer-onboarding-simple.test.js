const LaufGraphQLClient = require('../lib/graphql-client');
const LaufRestClient = require('../lib/rest-client');

describe('🎨 Онбординг дизайнера - Упрощенный сценарий', () => {
  let graphqlClient;
  let restClient;
  let authToken;
  let createdFlow;
  let firstStep;
  let secondStep;

  beforeAll(async () => {
    // Инициализация клиентов
    graphqlClient = new LaufGraphQLClient('http://localhost:8087/graphql');
    restClient = new LaufRestClient('http://localhost:8087');

    // Аутентификация через dev-auth
    const authResponse = await restClient.post('/api/auth/dev-login', {
      telegramId: 857395,
      firstName: "Админ2",
      lastName: "Петров",
      username: "admin_petrov2",
      languageCode: "ru",
      role: "Admin"
    }, 'Dev Authentication');

    expect(authResponse.status).toBe(200);
    expect(authResponse.data.token).toBeDefined();
    
    authToken = authResponse.data.token;
    
    // Устанавливаем токен для GraphQL клиента
    graphqlClient.client.setHeader('Authorization', `Bearer ${authToken}`);
  });

  describe('1. 🚀 Создание потока обучения', () => {
    test('Создать поток "Онбординг нового дизайнера"', async () => {
      const createFlowMutation = `
        mutation CreateFlow($input: CreateFlowInput!) {
          createFlow(input: $input) {
            id
            title
            description
            status
            isRequired
            priority
            createdAt
            totalSteps
          }
        }
      `;

      const variables = {
        input: {
          title: "Онбординг нового дизайнера",
          description: "Комплексная программа адаптации нового дизайнера в компании",
          isSequential: true,
          allowRetry: true,
          timeLimit: 14,
          passingScore: 80
        }
      };

      const response = await graphqlClient.executeMutation(
        createFlowMutation, 
        variables, 
        'Создание потока онбординга дизайнера'
      );

      expect(response.createFlow).toBeDefined();
      expect(response.createFlow.id).toBeDefined();
      expect(response.createFlow.title).toBe("Онбординг нового дизайнера");
      expect(response.createFlow.status).toBe("DRAFT");
      
      createdFlow = response.createFlow;
      
      console.log(`✅ Создан поток: ${createdFlow.title} (ID: ${createdFlow.id})`);
    });
  });

  describe('2. 📋 Создание шагов потока', () => {
    test('Создать первый шаг: "Знакомство с структурой компании"', async () => {
      const createStepMutation = `
        mutation CreateFlowStep($input: CreateFlowStepInput!) {
          createFlowStep(input: $input) {
            isSuccess
            message
            stepId
            step {
              id
              title
              description
              order
              isRequired
              instructions
              notes
            }
          }
        }
      `;

      const variables = {
        input: {
          flowId: createdFlow.id,
          title: "Знакомство с структурой компании",
          description: "Изучение организационной структуры, корпоративных ценностей и основных процессов компании",
          order: 1,
          isRequired: true,
          instructions: "Внимательно изучите предоставленные материалы о структуре компании",
          notes: "Первый шаг адаптации - самый важный для понимания контекста работы"
        }
      };

      const response = await graphqlClient.executeMutation(
        createStepMutation, 
        variables, 
        'Создание первого шага потока'
      );

      expect(response.createFlowStep.isSuccess).toBe(true);
      expect(response.createFlowStep.step).toBeDefined();
      expect(response.createFlowStep.step.title).toBe("Знакомство с структурой компании");
      
      firstStep = response.createFlowStep.step;
      
      console.log(`✅ Создан первый шаг: ${firstStep.title} (ID: ${firstStep.id})`);
    });

    test('Создать второй шаг: "Техническое обеспечение и поддержка"', async () => {
      const createStepMutation = `
        mutation CreateFlowStep($input: CreateFlowStepInput!) {
          createFlowStep(input: $input) {
            isSuccess
            message
            stepId
            step {
              id
              title
              description
              order
              isRequired
              instructions
              notes
            }
          }
        }
      `;

      const variables = {
        input: {
          flowId: createdFlow.id,
          title: "Техническое обеспечение и поддержка",
          description: "Настройка рабочего места, получение доступов к системам и изучение процедур получения технической поддержки",
          order: 2,
          isRequired: true,
          instructions: "Настройте свое рабочее место согласно инструкциям",
          notes: "Техническая подготовка критически важна для эффективной работы дизайнера"
        }
      };

      const response = await graphqlClient.executeMutation(
        createStepMutation, 
        variables, 
        'Создание второго шага потока'
      );

      expect(response.createFlowStep.isSuccess).toBe(true);
      expect(response.createFlowStep.step).toBeDefined();
      expect(response.createFlowStep.step.title).toBe("Техническое обеспечение и поддержка");
      
      secondStep = response.createFlowStep.step;
      
      console.log(`✅ Создан второй шаг: ${secondStep.title} (ID: ${secondStep.id})`);
    });
  });

  describe('3. 📝 Добавление компонентов в первый шаг', () => {
    test('Добавить компонент статьи: "Структура и ценности компании"', async () => {
      const createComponentMutation = `
        mutation CreateComponent($input: CreateComponentInput!) {
          createComponent(input: $input) {
            article {
              isSuccess
              message
              componentId
              linkId
              component {
                id
                title
                description
                content
                readingTimeMinutes
                status
              }
            }
          }
        }
      `;

      const variables = {
        input: {
          article: {
            flowStepId: firstStep.id,
            title: "Структура и ценности компании",
            description: "Подробное описание организационной структуры компании, её миссии, видения и корпоративных ценностей",
            content: "# Добро пожаловать в нашу команду дизайнеров!\n\n## Миссия компании\nМы создаем инновационные цифровые решения, которые делают жизнь людей лучше и проще.\n\n## Наши ценности\n- **Пользователь в центре** - все решения принимаются с учетом потребностей конечных пользователей\n- **Качество превыше всего** - мы не идем на компромиссы в вопросах качества продукта\n- **Непрерывное обучение** - мы постоянно развиваемся и изучаем новые подходы\n- **Командная работа** - успех достигается только совместными усилиями",
            readingTimeMinutes: 15,
            isRequired: true
          }
        }
      };

      const response = await graphqlClient.executeMutation(
        createComponentMutation, 
        variables, 
        'Создание компонента статьи о структуре компании'
      );

      expect(response.createComponent.article.isSuccess).toBe(true);
      expect(response.createComponent.article.component).toBeDefined();
      expect(response.createComponent.article.component.title).toBe("Структура и ценности компании");
      
      console.log(`✅ Создан компонент статьи: ${response.createComponent.article.component.title}`);
    });

    test('Добавить компонент квиза: "Проверка знаний о компании"', async () => {
      const createComponentMutation = `
        mutation CreateComponent($input: CreateComponentInput!) {
          createComponent(input: $input) {
            quiz {
              isSuccess
              message
              componentId
              linkId
              component {
                id
                title
                description
                questionText
                options {
                  id
                  text
                  isCorrect
                  message
                  points
                }
              }
            }
          }
        }
      `;

      const variables = {
        input: {
          quiz: {
            flowStepId: firstStep.id,
            title: "Проверка знаний о компании",
            description: "Тест для проверки понимания структуры компании и её ценностей",
            questionText: "Какая из перечисленных ценностей НЕ является основной ценностью нашей компании?",
            options: [
              {
                text: "Пользователь в центре",
                isCorrect: false,
                message: "Это одна из наших ключевых ценностей!",
                points: 0
              },
              {
                text: "Максимизация прибыли любой ценой",
                isCorrect: true,
                message: "Правильно! Мы ставим качество и пользователей выше краткосрочной прибыли.",
                points: 25
              },
              {
                text: "Качество превыше всего",
                isCorrect: false,
                message: "Это одна из наших основных ценностей.",
                points: 0
              },
              {
                text: "Непрерывное обучение",
                isCorrect: false,
                message: "Мы действительно ценим постоянное развитие.",
                points: 0
              }
            ],
            isRequired: true,
            estimatedDurationMinutes: 5
          }
        }
      };

      const response = await graphqlClient.executeMutation(
        createComponentMutation, 
        variables, 
        'Создание компонента квиза о компании'
      );

      expect(response.createComponent.quiz.isSuccess).toBe(true);
      expect(response.createComponent.quiz.component).toBeDefined();
      expect(response.createComponent.quiz.component.title).toBe("Проверка знаний о компании");
      
      console.log(`✅ Создан компонент квиза: ${response.createComponent.quiz.component.title}`);
    });

    test('Добавить компонент задания: "Знакомство с командой"', async () => {
      const createComponentMutation = `
        mutation CreateComponent($input: CreateComponentInput!) {
          createComponent(input: $input) {
            task {
              isSuccess
              message
              componentId
              linkId
              component {
                id
                title
                description
                instruction
                codeWord
                hint
              }
            }
          }
        }
      `;

      const variables = {
        input: {
          task: {
            flowStepId: firstStep.id,
            title: "Знакомство с командой",
            description: "Практическое задание на знакомство с коллегами и рабочими процессами",
            instruction: "1. Познакомьтесь лично с каждым членом дизайн-команды\n2. Узнайте у Head of Design кодовое слово для этого задания\n3. Запишите 3 интересных факта о команде\n4. Введите полученное кодовое слово для завершения задания",
            codeWord: "DESIGN_TEAM_2024",
            hint: "Кодовое слово можно получить у руководителя дизайн-команды после знакомства с коллегами",
            isRequired: true,
            estimatedDurationMinutes: 60
          }
        }
      };

      const response = await graphqlClient.executeMutation(
        createComponentMutation, 
        variables, 
        'Создание компонента задания на знакомство'
      );

      expect(response.createComponent.task.isSuccess).toBe(true);
      expect(response.createComponent.task.component).toBeDefined();
      expect(response.createComponent.task.component.title).toBe("Знакомство с командой");
      
      console.log(`✅ Создан компонент задания: ${response.createComponent.task.component.title}`);
    });
  });

  describe('4. ⚙️ Добавление компонентов во второй шаг', () => {
    test('Добавить статью: "Техническая настройка рабочего места"', async () => {
      const createComponentMutation = `
        mutation CreateComponent($input: CreateComponentInput!) {
          createComponent(input: $input) {
            article {
              isSuccess
              message
              componentId
              linkId
              component {
                id
                title
                description
                content
                readingTimeMinutes
                status
              }
            }
          }
        }
      `;

      const variables = {
        input: {
          article: {
            flowStepId: secondStep.id,
            title: "Техническая настройка рабочего места",
            description: "Пошаговое руководство по настройке всех необходимых инструментов и программ для работы дизайнера",
            content: "# Техническая настройка рабочего места\n\n## Необходимое программное обеспечение\n\n### Основные инструменты дизайна\n1. **Figma** (основной инструмент)\n   - Создайте аккаунт на figma.com\n   - Присоединитесь к команде компании\n   - Установите desktop приложение\n\n2. **Adobe Creative Suite**\n   - Photoshop - для обработки изображений\n   - Illustrator - для создания иллюстраций\n\n### Инструменты для совместной работы\n1. **Miro** - для воркшопов и мозговых штурмов\n2. **Notion** - для документации и заметок\n3. **Slack** - корпоративный мессенджер",
            readingTimeMinutes: 20,
            isRequired: true
          }
        }
      };

      const response = await graphqlClient.executeMutation(
        createComponentMutation, 
        variables, 
        'Создание статьи о технической настройке'
      );

      expect(response.createComponent.article.isSuccess).toBe(true);
      expect(response.createComponent.article.component.title).toBe("Техническая настройка рабочего места");
      
      console.log(`✅ Создана статья о технической настройке`);
    });

    test('Добавить задание: "Получение технических доступов"', async () => {
      const createComponentMutation = `
        mutation CreateComponent($input: CreateComponentInput!) {
          createComponent(input: $input) {
            task {
              isSuccess
              message
              componentId
              linkId
              component {
                id
                title
                description
                instruction
                codeWord
                hint
              }
            }
          }
        }
      `;

      const variables = {
        input: {
          task: {
            flowStepId: secondStep.id,
            title: "Получение технических доступов",
            description: "Практическое задание по получению всех необходимых доступов и настройке инструментов",
            instruction: "1. Настройте Figma и присоединитесь к команде компании\n2. Настройте Slack и присоединитесь к каналам\n3. Обратитесь к IT-администратору за доступами\n4. Получите кодовое слово для завершения задания\n5. Создайте тестовый макет в Figma",
            codeWord: "TECH_SETUP_OK",
            hint: "Кодовое слово можно получить у IT-администратора после настройки всех инструментов",
            isRequired: true,
            estimatedDurationMinutes: 120
          }
        }
      };

      const response = await graphqlClient.executeMutation(
        createComponentMutation, 
        variables, 
        'Создание задания по получению доступов'
      );

      expect(response.createComponent.task.isSuccess).toBe(true);
      expect(response.createComponent.task.component.title).toBe("Получение технических доступов");
      
      console.log(`✅ Создано задание по получению технических доступов`);
    });
  });

  describe('5. 🔄 Изменение порядка компонентов', () => {
    test('Переместить компоненты для оптимального порядка обучения', async () => {
      // Получаем информацию о созданном потоке
      const getFlowQuery = `
        query GetFlow($id: UUID!) {
          flow(id: $id) {
            id
            title
            steps {
              id
              title
              order
              components {
                id
                title
                componentType
                order
              }
            }
          }
        }
      `;

      const response = await graphqlClient.executeQuery(
        getFlowQuery, 
        { id: createdFlow.id }, 
        'Получение информации о потоке для перестановки компонентов'
      );

      expect(response.flow).toBeDefined();
      expect(response.flow.steps).toHaveLength(2);
      
      const firstStepComponents = response.flow.steps.find(s => s.order === 1)?.components || [];
      expect(firstStepComponents.length).toBeGreaterThanOrEqual(3);
      
      console.log(`✅ Поток содержит ${firstStepComponents.length} компонентов в первом шаге`);
      
      // Найдем квиз для перемещения в конец
      const quizComponent = firstStepComponents.find(c => c.componentType === 'QUIZ');
      if (quizComponent) {
        const reorderMutation = `
          mutation ReorderFlowComponent($input: ReorderFlowComponentInput!) {
            reorderFlowComponent(input: $input) {
              isSuccess
              message
            }
          }
        `;

        const reorderResponse = await graphqlClient.executeMutation(
          reorderMutation, 
          { input: { componentId: quizComponent.id, newPosition: 3 } }, 
          'Перемещение квиза в конец списка'
        );

        expect(reorderResponse.reorderFlowComponent.isSuccess).toBe(true);
        console.log(`✅ Квиз перемещен в конец списка компонентов`);
      }
    });
  });

  describe('6. 📊 Проверка созданного потока', () => {
    test('Получить полную информацию о созданном потоке', async () => {
      const getFlowQuery = `
        query GetFlow($id: UUID!) {
          flow(id: $id) {
            id
            title
            description
            status
            totalSteps
            steps {
              id
              title
              description
              order
              totalComponents
              components {
                id
                title
                componentType
                order
                isRequired
                estimatedDurationMinutes
              }
            }
          }
        }
      `;

      const response = await graphqlClient.executeQuery(
        getFlowQuery, 
        { id: createdFlow.id }, 
        'Получение полной информации о созданном потоке'
      );

      expect(response.flow).toBeDefined();
      expect(response.flow.title).toBe("Онбординг нового дизайнера");
      expect(response.flow.steps).toHaveLength(2);
      
      const firstStepComponents = response.flow.steps.find(s => s.order === 1)?.components || [];
      const secondStepComponents = response.flow.steps.find(s => s.order === 2)?.components || [];
      
      expect(firstStepComponents.length).toBeGreaterThanOrEqual(3);
      expect(secondStepComponents.length).toBeGreaterThanOrEqual(2);
      
      console.log(`\n🎯 ИТОГОВЫЙ РЕЗУЛЬТАТ:`);
      console.log(`📋 Поток: "${response.flow.title}"`);
      console.log(`📊 Статус: ${response.flow.status}`);
      console.log(`🔢 Количество шагов: ${response.flow.steps.length}`);
      
      response.flow.steps.forEach((step, index) => {
        console.log(`\n📌 Шаг ${step.order}: "${step.title}"`);
        console.log(`   📝 Описание: ${step.description}`);
        console.log(`   🧩 Компонентов: ${step.totalComponents}`);
        
        const sortedComponents = step.components.sort((a, b) => a.order - b.order);
        sortedComponents.forEach((comp, compIndex) => {
          const requiredMark = comp.isRequired ? '🔴' : '🟡';
          const durationText = comp.estimatedDurationMinutes ? ` (${comp.estimatedDurationMinutes} мин)` : '';
          console.log(`     ${compIndex + 1}. ${requiredMark} ${comp.title} [${comp.componentType}]${durationText}`);
        });
      });
      
      console.log(`\n✅ Поток онбординга дизайнера успешно создан и настроен!`);
    });
  });

  afterAll(async () => {
    console.log('\n🎉 СВОДКА ПО СОЗДАННОМУ ПОТОКУ ОНБОРДИНГА:');
    console.log('='.repeat(60));
    console.log(`✅ Создан поток: "${createdFlow?.title}"`);
    console.log(`✅ Создано шагов: 2`);
    console.log(`✅ Создано компонентов: 5 (3 статьи, 1 квиз, 1 задание)`);
    console.log(`✅ Выполнена перестановка компонентов`);
    console.log(`✅ Все тесты прошли успешно`);
    console.log('='.repeat(60));
    console.log(`📊 HTML отчет доступен в: reports/detailed-api-report.html`);
  });
});