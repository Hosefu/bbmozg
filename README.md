Миграции делаются командной
```
export PATH="$PATH:$HOME/.dotnet/tools" && dotnet ef migrations add RemoveEmailAndPhoneFields --project src/Lauf.Infrastructure --startup-project src/Lauf.Api
```