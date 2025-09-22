# evoltis-challenge
Desafío técnico de Back End para Evoltis desarrollado por Lisandro Guerrero

## Instrucciones para correr el proyecto

Modificar el ConnectionString dentro de `appsettings.json` para que coincida con el usuario, contraseña y puerto de la base de datos instalada localmente.
```
	"ConnectionStrings": {
		"DefaultConnection": "Server=localhost;Database=EvoltisChallenge;User=[USUARIO LOCAL];Password=[CONTRASEÑA LOCAL];Port=[PUERTO LOCAL];"
	},
```

Nos posicionamos sobre el directorio del proyecto principal
`cd [path_del_proyecto]/EvoltisChallenge/`
Restauramos los paquetes
`dotnet restore`
Buildeamos el proyecto
`dotnet build`
Aplicamos la migración de la base de datos
`dotnet ef database update`
Ejecutamos las pruebas
`dotnet test`
Ejecutamos la API
`dotnet run`

La documentación de la API se encuentra en: http://localhost:5292/index.html