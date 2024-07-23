# Oscallo Castle AddonedKernel

Дополнение к библиотеке Castle.Windstor, позволяющее добавлять вызываемые автоматически методы на основе интерфейсов.

## Содержание
- [Предисловие](#предисловие)
- [Лицензия](#лицензия)
- [Релизы](#релизы)
- [Установка](#установка)
- [Использование](#использование)
- [Технологии](#технологии)
- [Поддержка проекта](#Поддержка-проекта)
- [Команда проекта](#команда-проекта)

## Предисловие
Не является официальной разработкой проекта [Castle Project](https://github.com/castleproject).

## Лицензия
© 2024 Oscallo. Это бесплатное программное обеспечение, распространяемое в соответствии с лицензией [Apache 2.0](https://github.com/Oscallo/Castle.AddonedKernel?tab=Apache-2.0-1-ov-file).

## Релизы 
Смотрите [Релизы](https://github.com/oscallo/castle.addonedkernel/releases)

## Установка

Варианты установки (опциональны):

.NET Cli
``` 
dotnet add package Oscallo.Castle.AddonedKernel --version X.X.X
```

Package Manager
```
PM>NuGet\Install-Package Oscallo.Castle.AddonedKernel -Version X.X.X
```

PackageReference
``` xml
<PackageReference Include="Oscallo.Castle.AddonedKernel" Version="X.X.X" />
```

Paket Cli
```
paket add Oscallo.Castle.AddonedKernel --version X.X.X
```

Script & Interactive
```
#r "nuget: Oscallo.Castle.AddonedKernel, X.X.X"
```

Cake
```
// Install Oscallo.Castle.AddonedKernel as a Cake Addin
#addin nuget:?package=Oscallo.Castle.AddonedKernel&version=X.X.X

// Install Oscallo.Castle.AddonedKernel as a Cake Tool
#tool nuget:?package=Oscallo.Castle.AddonedKernel&version=X.X.X
```

## Использование

1. [Установка](#установка) библиотеки выбранным способом

2. Необходимо реализовать класс, наследуемый ```IInjector```. Опционально возможно реализовать классы, наследуемые от ```IResolver``` и ```IRegistrar```


``` cs
public class Injector : IInjector
{
	//...
}
```

или же 

``` cs
public class Registrar : IRegistrar
{
	//...
}

public class Resolver : IResolver
{
	//...
}
```

3. Создать экземпляр контейнера 

``` cs
WindsorContainer container = new();
```

4. Для простоты рекомендуется реализовать в классах из пункта 2 статических метода

``` cs
public static void Register(IWindsorContainer windsorContainer, params IRegistration[] registrations)
{
	windsorContainer.Register(registrations);
}

public static T Resolve<T>(IWindsorContainer windsorContainer)
{
	return windsorContainer.Resolve<T>();
}
```

Подготовительный этап закончен.  

5. Необходимо создать ```Builder```:

5.1 Создадим список будущих методов, вызываемых у интерфейса.

``` cs
List<CalleableMethodInfo> methodInfos = new();
```
5.2 Необходимо заполнить информацию о методе для типа:

5.2.1 Создаем массив объектов, в массив передаем необходимые аргументы метода

Далее разберем на основе интерфейса автоматической интеграции.
Для автоматической интеграции необходим сам объект класса, наследуемого от ```IRegistrar```, он и передается в массив. 
``` cs
object[] parametersArray = new object[] { injector };
```

5.2.2 Создаем информацию о методе
```IIntegrator``` - контракт, гарантирующий создание метода регистрации зависимостей, он содержит лишь один метод ```void Integrate(IRegistrar injector)```
``` cs
MethodInfo methodInfo = typeof(IIntegrator).GetMethod(nameof(IIntegrator.Integrate));
```

5.2.3 Создаем структуру, описывающую алгоритм действий, для ```Builder```

``` cs
CalleableMethodInfo calleableMethodInfo = new() { MethodCallTypeEnum = CalleableMethodCallTypeEnum.PreResolve, MethodTypeEnum = CalleableMethodTypeEnum.Examplar, MethodInfo = methodInfo, Arguments = parametersArray, MethodArgumentsEnum = CalleableMethodArgumentsEnum.FromCalleabeMethod };
```
Далее разберем, что было передано при создании  
```MethodCallTypeEnum``` - Сущность, указывающая когда будет выполнен метод.  
```MethodTypeEnum``` - Сущность, указывающая где будет выполнен метод.   
```MethodInfo``` - информация о методе из пункта 5.2.2  
```Arguments``` - аргументы метода из пункта 5.2.1  
```MethodArgumentsEnum``` - сущность, отвечающая за преобразование аргументов.  

Имеет 3 состояния:  
```CalleableMethodArgumentsEnum.FromInjector``` - использование аргументов, в чистом виде из ```Arguments``` 
```CalleableMethodArgumentsEnum.FromInjectorWithComponent``` - аналогично ```CalleableMethodArgumentsEnum.FromInjector```, только сам компонент подставляется первым аргументом.  
```CalleableMethodArgumentsEnum.FromCalleabeMethod``` - использование аргументов, подобраных активатором  

5.2.4 Добавляем это в ранее созданный в пункте 5.1 список

``` cs
methodInfos.Add(calleableMethodInfo);
```

5.2.5 Создаем ```Builder``` или же добавляем в него новые элементы
``` cs
if (builder == null)
{
	return new Builder(new BuilderElement(typeof(IIntegrator), methodInfos));
}
else
{
	builder.Add(new BuilderElement(typeof(IIntegrator), methodInfos));
}
```
Cам ```Builder``` хранится в статическом поле ```BuildeableComponentActivatorFacility.Builder```

5.2.6 Добавляем	```BuildeableComponentActivatorFacility```

``` cs
injector.AddFacilityIfAbsent<BuildeableComponentActivatorFacility>();
```

## Технологии
- [Castle Windsor](http://www.castleproject.org/)
- [C#](https://dotnet.microsoft.com/ru-ru/languages/csharp)
- [Dependency injection](https://en.wikipedia.org/wiki/Dependency_injection)

## Поддержка проекта
Прежде всего, спасибо, что нашли время внести свой вклад!

Все виды вкладов приветствуются и ценятся. Пожалуйста, обязательно прочтите соответствующий раздел, прежде чем вносить свой вклад. Это значительно облегчит задачу для нас, сопровождающих, и сгладит опыт для всех участников. Сообщество с нетерпением ждет ваших вкладов.

> И если вам нравится проект, но у вас просто нет времени вносить свой вклад, это нормально. Есть и другие простые способы поддержать проект и выразить свою признательность, которым мы также будем очень рады:
> 
> - Звезда проекта
> - Твитнуть об этом
> - Укажите этот проект в файле readme вашего проекта
> - Расскажите о проекте на местных встречах и друзьям/коллегам.

### Зачем мы разработали этот проект?
Сначала этот проект разрабатывался с целью разгрузки больших фрагментов кода, которые появлялись при использовании Dependency Injection. Потом было принято решение не останавливаться только на передаче ответственности за регистрацию зависимостей на объект. Так была добавлена возможность использовать автоматической проверки объекта зависимости на наличие интерфейсов и автоматический вызов указанных методов.

## Команда проекта
Связаться со мной возможно через [Telegram](https://web.telegram.org/)

- [Oscallo](https://t.me/zero_indefined)

