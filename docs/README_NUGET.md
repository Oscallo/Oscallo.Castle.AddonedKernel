# Oscallo Castle AddonedKernel

RU: Дополнение к библиотеке Castle.Windstor, позволяющее добавлять вызываемые автоматически методы на основе интерфейсов.
EN: Extension to the Castle.Windstor library is enabled by automatically called interface-based methods.

## Пример использования (Example of use)

RU: 1. Необходимо реализовать класс, наследуемый ```IInjector```. Опционально возможно реализовать классы, наследуемые от ```IResolver``` и ```IRegistrar```  
EN: 1. You need to implement a class that inherits from ```IInjector```. Optionally, it is possible to implement classes that inherit from ```IResolver``` and ```IRegistrar```

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

RU: 2. Создать экземпляр контейнера   
EN: 2. Create a container instance

``` cs
WindsorContainer container = new();
```

RU: 3. Для простоты рекомендуется реализовать в классах из пункта 2 статических метода  
EN: 3. For simplicity, it is recommended to implement static methods in classes from point 2

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

4. Необходимо создать ```Builder```:

4.1 Создадим список будущих методов, вызываемых у интерфейса.  

EN: The preparatory stage is over.  

4. You need to create a ```Builder```:

4.1 Let's create a list of future methods called on the interface.

``` cs
List<CalleableMethodInfo> methodInfos = new();
```
RU: 4.2 Необходимо заполнить информацию о методе для типа:

4.2.1 Создаем массив объектов, в массив передаем необходимые аргументы метода

Далее разберем на основе интерфейса автоматической интеграции.
Для автоматической интеграции необходим сам объект класса, наследуемого от ```IRegistrar```, он и передается в массив.   

EN: 4.2 It is necessary to fill in the method information for the type:

4.2.1 Create an array of objects, pass the necessary method arguments to the array

Next, we will analyze it based on the automatic integration interface.
For automatic integration, the object of the class itself, inherited from ```IRegistrar```, is required, and it is transferred to the array.

``` cs
object[] parametersArray = new object[] { injector };
```

RU: 4.2.2 Создаем информацию о методе
```IIntegrator``` - контракт, гарантирующий создание метода регистрации зависимостей, он содержит лишь один метод ```void Integrate(IRegistrar injector)```  

EN: 4.2.2 Create information about the method
```IIntegrator``` is a contract that guarantees the creation of a dependency registration method; it contains only one method ```void Integrate(IRegistrar injector)```

``` cs
MethodInfo methodInfo = typeof(IIntegrator).GetMethod(nameof(IIntegrator.Integrate));
```

RU: 4.2.3 Создаем структуру, описывающую алгоритм действий, для ```Builder```  

EN: 4.2.3 Create a structure describing the algorithm of actions for ```Builder``

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

4.2.4 Добавляем это в ранее созданный в пункте 5.1 список

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

4.2.4 Add this to the list previously created in paragraph 5.1

``` cs
methodInfos.Add(calleableMethodInfo);
```

RU: 4.2.5 Создаем ```Builder``` или же добавляем в него новые элементы  

EN: 4.2.5 Create a ```Builder``` or add new elements to it

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

4.2.6 Добавляем	```BuildeableComponentActivatorFacility```  

EN: The ```Builder``` itself is stored in the static field ```BuildeableComponentActivatorFacility.Builder```

4.2.6 Add ```BuildeableComponentActivatorFacility

``` cs
injector.AddFacilityIfAbsent<BuildeableComponentActivatorFacility>();
```

## Лицензия (License)
RU: © 2024 Oscallo. Это бесплатное программное обеспечение, распространяемое в соответствии с лицензией [Apache 2.0](https://github.com/Oscallo/Castle.AddonedKernel?tab=Apache-2.0-1-ov-file).  
EN: © 2024 Oscallo. This is free software distributed under the [Apache 2.0] license(https://github.com/Oscallo/Castle.AddonedKernel?tab=Apache-2.0-1-ov-file).
