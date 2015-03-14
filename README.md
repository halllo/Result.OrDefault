Result.OrDefault
================
C#6 will support null propagation (https://github.com/dotnet/roslyn/wiki/Languages-features-in-C%23-6-and-VB-14).
Result.OrDefault is simple null propagation for pre-C#6.

Do you know requirements like "display the employees name in the invoice view"? Then probably your ViewModel code looks a lot like this.
```csharp
public string EmployeeName
{
   get 
   {
      if (SelectedItem != null && SelectedItem.Invoice != null && SelectedItem.Invoice.Employee != null && SelectedItem.Invoice.Employee.Name != null)
      {
         return SelectedItem.Invoice.Employee.Name.ToString();
      }
      else
      {
         return string.Empty;
      }
   }   
}
```
The requirements code is cluttered in null checks, since you dont want the user to get NullReferenceExceptions when no invoice is selected or the selected invoice does not have an employee yet and so on.
If you want to change the code to not use the Employee property anymore but to use the Sender property, make sure to change the two null checks as well.
Disadvantage of the above code is that the actual requirement is hidden in safeguard-code.
There are multiple layers of abstraction and much code to maintain. 
And this is just a simple example. 
What would you have had to do if the sample had used methods with side effects instead of properties? 
Sure, CQRS but lets be honest.
In order to not invoke a method twice, local variables are needed and your IF with a few conditions very quickly turns into a bloated mess.

I dont want to write all that safeguard-code since it blurs the view on what was originally required and makes it difficult to see why the code was written in the first place.
I just want to write what the requirement is.
```csharp
public string EmployeeName
{
   get 
   {
      return SelectedItem.Invoice.Employee.Name.ToString();
   }
}
```
Yes, that would not work.
In C#6 we could write it like this.
```csharp
public string EmployeeName
{
   get
   {
      return SelectedItem?.Invoice?.Employee?.Name?.ToString();
   }
}
```
But what if a system is stuck on C#5?
Result.OrDefault lets you write the requirement like this.
```csharp
public string EmployeeName
{
   get 
   {
      return Result.OrDefault(() => SelectedItem.Invoice.Employee.Name.ToString());
   }
}
```
Result.OrDefault uses reflection to invoke one step at a time, checks for null and invokes the next step if not null (it only takes twice as long as with IFs for null checks).
It offers a much cleaner syntax since the focus is on the requirement and not on low level safeguard-code.