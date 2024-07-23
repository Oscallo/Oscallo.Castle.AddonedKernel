# Oscallo Castle AddonedKernel

![NuGet Downloads](https://img.shields.io/nuget/dt/Oscallo.Castle.AddonedKernel)
![NuGet Version](https://img.shields.io/nuget/v/Oscallo.Castle.AddonedKernel)

RU: Дополнение к библиотеке Castle.Windstor, позволяющее добавлять вызываемые автоматически методы на основе интерфейсов.  
EN: An addition to the Castle.Windstor library that allows you to add automatically called methods based on interfaces.

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
RU: Не является официальной разработкой проекта [Castle Project](https://github.com/castleproject).  
EN: This is not an official development of the [Castle Project](https://github.com/castleproject).

## Лицензия
RU: © 2024 Oscallo. Это бесплатное программное обеспечение, распространяемое в соответствии с лицензией [Apache 2.0](https://github.com/Oscallo/Castle.AddonedKernel?tab=Apache-2.0-1-ov-file).  
EN: © 2024 Oscallo. This is free software distributed under the [Apache 2.0] license(https://github.com/Oscallo/Castle.AddonedKernel?tab=Apache-2.0-1-ov-file).

## Релизы 
RU: Смотрите [Релизы](https://github.com/oscallo/castle.addonedkernel/releases)  
EN: See [Releases](https://github.com/oscallo/castle.addonedkernel/releases)

## Установка

RU: Варианты установки (опциональны):  
EN: Installation options (optional):

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

[![NuGet version (SoftCircuits.Silk)](https://img.shields.io/nuget/v/SoftCircuits.Silk.svg?style=flat-square)](https://www.nuget.org/packages/SoftCircuits.Silk/)

RU: 1. [Установка](#установка) библиотеки выбранным способом  
EN: 1. [Install](#установка) libraries in the chosen way

RU: 2. Необходимо реализовать класс, наследуемый ```IInjector```. Опционально возможно реализовать классы, наследуемые от ```IResolver``` и ```IRegistrar```  
EN: 2. You need to implement a class that inherits from ```IInjector```. Optionally, it is possible to implement classes that inherit from ```IResolver``` and ```IRegistrar```

``` cs
public class Injector : IInjector
{
	//...
}
```

RU: или же   
EN: or

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

RU: 3. Создать экземпляр контейнера  
EN: 3. Create a container instance

``` cs
WindsorContainer container = new();
```

RU: 4. Для простоты рекомендуется реализовать в классах из пункта 2 статических метода  
EN: 4. For simplicity, it is recommended to implement static methods in classes from point 2

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

RU: Подготовительный этап закончен.  

5. Необходимо создать ```Builder```:

5.1 Создадим список будущих методов, вызываемых у интерфейса.  

EN: The preparatory stage is over.  

5. You need to create a ```Builder```:

5.1 Let's create a list of future methods called on the interface.

``` cs
List<CalleableMethodInfo> methodInfos = new();
```
RU: 5.2 Необходимо заполнить информацию о методе для типа:

5.2.1 Создаем массив объектов, в массив передаем необходимые аргументы метода

Далее разберем на основе интерфейса автоматической интеграции.
Для автоматической интеграции необходим сам объект класса, наследуемого от ```IRegistrar```, он и передается в массив.   

EN: 5.2 It is necessary to fill in the method information for the type:

5.2.1 Create an array of objects, pass the necessary method arguments to the array

Next, we will analyze it based on the automatic integration interface.
For automatic integration, the object of the class itself, inherited from ```IRegistrar```, is required, and it is transferred to the array.

``` cs
object[] parametersArray = new object[] { injector };
```

RU: 5.2.2 Создаем информацию о методе
```IIntegrator``` - контракт, гарантирующий создание метода регистрации зависимостей, он содержит лишь один метод ```void Integrate(IRegistrar injector)```  

EN: 5.2.2 Create information about the method
```IIntegrator``` is a contract that guarantees the creation of a dependency registration method; it contains only one method ```void Integrate(IRegistrar injector)```

``` cs
MethodInfo methodInfo = typeof(IIntegrator).GetMethod(nameof(IIntegrator.Integrate));
```

RU: 5.2.3 Создаем структуру, описывающую алгоритм действий, для ```Builder```  

EN: 5.2.3 Create a structure describing the algorithm of actions for ```Builder``

``` cs
CalleableMethodInfo calleableMethodInfo = new() { MethodCallTypeEnum = CalleableMethodCallTypeEnum.PreResolve, MethodTypeEnum = CalleableMethodTypeEnum.Examplar, MethodInfo = methodInfo, Arguments = parametersArray, MethodArgumentsEnum = CalleableMethodArgumentsEnum.FromCalleabeMethod };
```
RU: Далее разберем, что было передано при создании  
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

EN: Next, let's look at what was transferred during creation.  
```MethodCallTypeEnum``` - An entity indicating when the method will be executed.  
```MethodTypeEnum``` - An entity indicating where the method will be executed.   
```MethodInfo``` - information about the method from clause 5.2.2  
```Arguments``` - method arguments from clause 5.2.1  
```MethodArgumentsEnum``` - the entity responsible for converting arguments.  

Has 3 states:  
```CalleableMethodArgumentsEnum.FromInjector``` - use of arguments, in their pure form from ```Arguments``` 
```CalleableMethodArgumentsEnum.FromInjectorWithComponent``` - similar to ```CalleableMethodArgumentsEnum.FromInjector```, only the component itself is substituted as the first argument.  
```CalleableMethodArgumentsEnum.FromCalleabeMethod``` - use of arguments picked up by the activator  

5.2.4 Add this to the list previously created in paragraph 5.1

``` cs
methodInfos.Add(calleableMethodInfo);
```

RU: 5.2.5 Создаем ```Builder``` или же добавляем в него новые элементы  

EN: 5.2.5 Create a ```Builder``` or add new elements to it

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
RU: Cам ```Builder``` хранится в статическом поле ```BuildeableComponentActivatorFacility.Builder```

5.2.6 Добавляем	```BuildeableComponentActivatorFacility```  

EN: The ```Builder``` itself is stored in the static field ```BuildeableComponentActivatorFacility.Builder```

5.2.6 Add ```BuildeableComponentActivatorFacility

``` cs
injector.AddFacilityIfAbsent<BuildeableComponentActivatorFacility>();
```

## Технологии
- [Castle Windsor](http://www.castleproject.org/)
- [C#](https://dotnet.microsoft.com/ru-ru/languages/csharp)
- [Dependency injection](https://en.wikipedia.org/wiki/Dependency_injection)

## Поддержка проекта
RU: Прежде всего, спасибо, что нашли время внести свой вклад!

Все виды вкладов приветствуются и ценятся. Пожалуйста, обязательно прочтите соответствующий раздел, прежде чем вносить свой вклад. Это значительно облегчит задачу для нас, сопровождающих, и сгладит опыт для всех участников. Сообщество с нетерпением ждет ваших вкладов.

> И если вам нравится проект, но у вас просто нет времени вносить свой вклад, это нормально. Есть и другие простые способы поддержать проект и выразить свою признательность, которым мы также будем очень рады:
> 
> - Звезда проекта
> - Твитнуть об этом
> - Укажите этот проект в файле readme вашего проекта
> - Расскажите о проекте на местных встречах и друзьям/коллегам.  

EN: First of all, thank you for taking the time to contribute!

All types of contributions are welcomed and appreciated. Please be sure to read the relevant section before contributing. This will make things much easier for us escorts and smooth out the experience for everyone involved. The community looks forward to your contributions.

>And if you like the project but just don't have time to contribute, that's okay. There are other simple ways to support the project and express your gratitude, which we will also be very happy to use:
> 
> - Star of the project
> - Tweet about it
> - Specify this project in your project's readme file
> - Tell your friends/colleagues about the project at local meetings.

### Зачем мы разработали этот проект?
RU: Сначала этот проект разрабатывался с целью разгрузки больших фрагментов кода, которые появлялись при использовании Dependency Injection. Потом было принято решение не останавливаться только на передаче ответственности за регистрацию зависимостей на объект. Так была добавлена возможность использовать автоматической проверки объекта зависимости на наличие интерфейсов и автоматический вызов указанных методов.  
EN: This project was initially developed with the goal of offloading large pieces of code that appeared when using Dependency Injection. Then it was decided not to stop only at transferring responsibility for registering dependencies to the object. Thus, the ability to use automatic checking of a dependency object for the presence of interfaces and automatic calling of specified methods was added.

## Команда проекта
RU: Связаться со мной возможно через на Github  
EN: You can contact me via on Github
