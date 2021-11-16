# Masiv Code Ruleta - Api
Este proyecto representa una ruleta de apuestas online

**Estas instrucciones te permitirán obtener una copia del proyecto en funcionamiento en tu máquina local para propósitos de desarrollo y pruebas.**

**Pre-requisitos**

* .Net Core 3.1

- Opcionales
    1. SourceTree (Cliente para manejo de git)
    	- 1.1 Personal acceso al proyecto    
    2. Visual studio 2019

**Instalación**

1. Instalar SDK .Net Core 3.1
2. Abrir la solución en visual studio 2019 (Recomendado)
    
- Opcional
1. Descargar el cliente SourceTree
2. Ejecutar SourceTree
	- 2.1. Solicitar permisos para realizar cambios al proyecto enviando un correo a yac8807@gmail.com 
    	- 2.2. Agregar su cuenta de GitHub con tu usario (correo electrónico) e ingresar el password
    	- 2.3. Clonar el proyecto en su maquina local
    	- 2.4. Inicializar git flow
	
**Variables de entorno**	
1. Debe validar los valores que contienen las variables de entorno del proyecto
	- 1.1 Haga clicl derecho en el proyercto y escoja la opción de propiedas
	- 1.2 Dirijase a la sección Debug
	- 1.3 Tenga encuenta el valor de las varaibles Username y Password
	- Opcional
	- 1.4 Cambie los valores de ser necesario

**Despliegue**

1. Hacer commit de los cambios realizados en la rama.
2. Antes de finalizar la rama, hacer pull para obtener los cambios de la rama develop y compilar el proyecto.
3. Una vez finalizado el feature se debe hacer push en la rama develop.

**Compilación**
1. Abra la solución en Visual Studio 2019 (**Preferiblemente**)
2. En la pestaña *Solution Explorer (Explorador de la solución)* haga click derecho sobre la solución y seleccione la opción *Clean (Limpiar)*
3. En la pestaña *Solution Explorer (Explorador de la solución)* haga click derecho sobre la solución y seleccione la opción *Build Solution (Compilar)*
4. Haga click en el botón Play(IIS Express) o oprima la tecla F5
5. Espere que se compile la solución y abra la ventana de Swagger
6. En la ventana desplegada del navegador haga click en el botón Authorize
7. Agregue el usuario y contraseña correcto
8. Consulte los diferentes métodos

**Construido con**

* .Net Core 3.1

**Autores**

* Yoe Cardenas - Desarrollador full stack
