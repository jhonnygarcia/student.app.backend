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