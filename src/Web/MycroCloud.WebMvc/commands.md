**Publish**

```
dotnet publish -c Release -o publish /p:UseAppHost=false
```
**Copy**
```
scp -i ~/.ssh/keys/npham.me-nam -r publish/* nam@npham.me:/home/nam/workspaces/mockserver/web/
```

**Build tailwindcss**
```
npx tailwindcss -i tailwind.input.css -o wwwroot/css/tailwind.css
```

**Install library with LibMan**
```
libman install vue@3.3.4 --provider cdnjs --destination wwwroot/lib/vue --files vue.global.min.js
```