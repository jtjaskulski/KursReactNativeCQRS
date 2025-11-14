# Lekcja 8: Walidacja - FluentValidation + Pipeline Behaviors (2 godziny)

**Moduł:** .NET Validation, DevOps dla API  
**Czas trwania:** 2 godziny

---

## 🎯 Cele Lekcji

Po ukończeniu tej lekcji będziesz potrafić:
- ✅ Wdrożyć walidację biznesową FluentValidation
- ✅ Utworzyć walidatory dla Commands Queries
- ✅ Skonfigurować ValidationBehavior w MediatR Pipeline
- ✅ Obsłużyć błędy walidacji globalnie
- ✅ Przekazywać błędy do klienta (React Native)

---

## CZĘŚĆ 1: Instalacja FluentValidation (15 minut)

### 1.1. Instalacja

```powershell
Install-Package FluentValidation.AspNetCore -Version 11.7.0
Install-Package FluentValidation.DependencyInjectionExtensions
```

---

## CZĘŚĆ 2: Tworzymy walidatory (25 minut)

### 2.1. Przykład walidatora dla CreateItemCommand

**Features/Items/Commands/CreateItem/CreateItemValidator.cs:**
```csharp
using FluentValidation;

namespace SolutionOrdersReact.Server.Features.Items.Commands.CreateItem
{
    public class CreateItemValidator : AbstractValidator<CreateItemCommand>
    {
        public CreateItemValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Nazwa jest wymagana")
                .MaximumLength(200);
            RuleFor(x => x.IdCategory)
                .GreaterThan(0).WithMessage("Wybierz kategorię");
            RuleFor(x => x.Price)
                .NotNull().WithMessage("Cena jest wymagana")
                .GreaterThanOrEqualTo(0);
            RuleFor(x => x.Quantity)
                .NotNull().WithMessage("Ilość jest wymagana")
                .GreaterThanOrEqualTo(0);
        }
    }
}
```

---

## CZĘŚĆ 3: Pipeline Behavior dla MediatR (30 minut)

### 3.1. Wzór ValidationBehavior

**Behaviors/ValidationBehavior.cs:**
```csharp
using FluentValidation;
using MediatR;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any()) return await next();

        var context = new ValidationContext<TRequest>(request);
        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
            throw new ValidationException(failures);

        return await next();
    }
}
```

### 3.2. Rejestracja pipeline w Program.cs

```csharp
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
```

---

## CZĘŚĆ 4: Globalna obsługa błędów (20 minut)

### 4.1. Middleware ExceptionHandler

**Middlewares/ExceptionMiddleware.cs:**
```csharp
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    public ExceptionMiddleware(RequestDelegate next) => _next = next;
    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (FluentValidation.ValidationException vex)
        {
            httpContext.Response.StatusCode = 400;
            httpContext.Response.ContentType = "application/json";
            var errors = vex.Errors.Select(e => e.ErrorMessage).ToArray();
            var body = System.Text.Json.JsonSerializer.Serialize(new { errors });
            await httpContext.Response.WriteAsync(body);
        }
        catch (Exception ex)
        {
            httpContext.Response.StatusCode = 500;
            await httpContext.Response.WriteAsync("Błąd wewnętrzny serwera");
        }
    }
}
```

**Rejestracja w Program.cs**
```csharp
app.UseMiddleware<ExceptionMiddleware>();
```

---

## CZĘŚĆ 5: Wyświetlanie błędów w React Native (30 minut)

### 5.1. Przykład obsługi walidacji przy CreateItem

```typescript
try {
  await apiService.createItem(data);
  Alert.alert('Sukces', 'Produkt utworzony!');
} catch (err) {
  // err.message może być "HTTP 400: ..."
  if (err instanceof Error && err.message.includes('400')) {
    const errors = JSON.parse(err.message.replace(/^HTTP 400: /, '')).errors;
    Alert.alert('Błąd walidacji', errors.join('\n'));
  } else {
    Alert.alert('Błąd', err.message || 'Unknown error');
  }
}
```

---

## 📝 Zadania praktyczne

### Zadanie 1: Napisać walidator i testować błędy dla CreateOrderCommand
### Zadanie 2: Przekazać błędy z backend do formularza CreateOrder w mobilce
### Zadanie 3: Rozszerzyć walidację o unikatowy kod produktu (Code)

---

## ➡️ Następna Lekcja

**[Lekcja 9: Zaawansowane CQRS – Behaviors, Logging, Paginacja](./lekcja-09-zaawansowane-cqrs.md)**

---

**Gratulacje! Potrafisz wdrożyć walidację CQRS end-to-end!**
