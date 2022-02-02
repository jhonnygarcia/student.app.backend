# Students App

La aplicación expone servicios web api para el CRUD de las siguientes entidades:
- Students (Estudiantes)
- Teachers (Profesores)
- Scores (Puntuaciones/Calificaciones)
- Classes (Clases)
- Subjects (Asignaturas)

## Dependencias del proyecto

- dotnet 5.0
- SQL Express/SQL Server
- EntityFrameworkCore
- AutoMapper


## Para ejecutar el proyecto
1.- Como paso inicial verificar que la cadena de conexión es válida para su equipo en caso uso la que es con autenticación de Windows: 

    Server=(localdb)\\MSSQLLocalDB;Database=StudentDb;Trusted_Connection=True;

2.- Ejecutar mediante su la consola de nugget el siguiente comando que aplica todos los migrations: 

    Update-Database -verbose

3.- Ejecutar la aplicacion una vez, una vez inicie la aplicacion se brindara la documentación que ofrece el swagger


## Los servicios se los puede consumir de 2 formas: enviando una cabecera de nombre X-Api-Key o enviando un token previamente generado

1.- Mediante un ApiKey, en el appsettings.json existe un bloque donde se encuentran estos, esta es una forma de proteger los servicios pero solo dentro de una red segura ya sea en el mismo servidor o clientes de confianza.
```json
  "Security": {
    "AuthorizedApiKeys": [
      {
        "Name": "Unidefined1",
        "ApiKey": "a754432e-6db3-4ace-b6bc-fb6ca49987ab"
      },
      {
        "Name": "Unidefined2",
        "ApiKey": "742cbe84-cbf4-4400-b0d2-2ad847f69861"
      }
    ]
  }
```

He aqui un ejemplo:

    curl -X 'GET' \
    'https://localhost:5001/api/v1/students' \
    -H 'accept: application/json' \
    -H 'X-Api-Key: a754432e-6db3-4ace-b6bc-fb6ca49987ab'

2.- Mediante una solicitud det tipo POST a la ruta /auth/login enviando por el cuerpo un JSON como el que sigue:
  
  ```json
  {
    "login": "juanp",
    "password": "123"
  }
  ```

la respuesta tendria esta forma

  ```json
  {
    "token_type": "bearer",
    "access_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIxIiwidXBuIjoiSnVhbiBQZXJleiIsInVuaXF1ZV9uYW1lIjoiSnVhbiBQZXJleiIsIm5iZiI6MTY0MzgxMDE4MywiZXhwIjoxNjQ",
    "expire_in": 3600
  }
  ```
Para tener un vistazo de los usuarios y sus contraseñas se implemento una autenticación contra unos datos planos en el appsettings.json

```json
"BasicAuthentication": [
    {
        "Id": "1",
        "Name": "Juan Perez",
        "Login": "juanp",
        "Password": "123"
    },
    {
        "Id": "2",
        "Name": "Dario Lopez",
        "Login": "dariol",
        "Password": "456"
    }
]
```

He aqui un ejemplo

    curl -X 'GET' \
    'https://localhost:5001/api/v1/students' \
    -H 'accept: application/json' \
    -H 'Authorization: Bearer BQZXJleiIsInVuaXF1ZV9uYW1lIjoiSnVhbiBQZXJleiIsIm5iZiI6MTY0MzgxMDE4MywiZXhwIjoxNjQ'

## Para poder fasilitar la carga de datos existen dos servicios que se pueden usar:

1.- Para cargar datos de prueba:

```curl
curl -X 'POST' \
  'https://localhost:5001/api/v1/generate-test-data' \
  -H 'accept: application/octet-stream' \
  -d ''
```

2.- Para limpiar toda la base de datos:

```curl
curl -X 'POST' \
  'https://localhost:5001/api/v1/clean-all-data' \
  -H 'accept: application/octet-stream' \
  -d ''
```