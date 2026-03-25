# 🥊 ITBrawl - Unity 3D Brawler & Web Infrastructure

[![Unity](https://img.shields.io/badge/Unity-100000?style=for-the-badge&logo=unity&logoColor=white)](https://unity.com/)
[![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Linux Server](https://img.shields.io/badge/Linux-FCC624?style=for-the-badge&logo=linux&logoColor=black)]()

> Un videojuego de lucha 3D desarrollado en Unity que implementa una arquitectura robusta basada en **Máquinas de Estados (State Machines)**, acompañado de una infraestructura de red con DMZ y servidor web seguro.

## 🚀 Sobre el Proyecto

**ITBrawl** es un proyecto desarrollado por el equipo InTerBits. El objetivo principal ha sido crear un brawler funcional desde cero, abarcando todas las fases del desarrollo: desde el modelado 3D y la grabación de audio personalizada, hasta la programación avanzada de físicas y estados de los personajes, culminando en la configuración de una infraestructura de red para su despliegue web.

Este repositorio contiene el código fuente del juego y la documentación técnica de la infraestructura.

## 💻 Habilidades Técnicas y Arquitectura Destacada

Este proyecto demuestra conocimientos avanzados en varias áreas clave del desarrollo de software y sistemas:

### 1. Programación Orientada a Objetos y Patrones de Diseño (C#)
El control de los personajes se ha construido utilizando el **Patrón de Diseño State Machine** para garantizar un código limpio, escalable y libre de bugs (evitando las complejas cadenas de `if/else`). 
* **`PlayerStateManager.cs`**: Gestor principal que controla las transiciones entre estados.
* **Estados modulares**: Implementación de clases independientes para cada comportamiento del jugador (`IdleState.cs`, `WalkingState.cs`, `JumpState.cs`, `AirState.cs`, `AttackingState.cs`).
* **Sistema de Combate**: Gestión de hitboxes y daño a través de `CharacterAttacks.cs`.

### 2. Desarrollo Multimedial y Unity
* **Integración 3D**: Implementación de modelos 3D y mapas personalizados (Mapa Nuvulet).
* **Animaciones Sincronizadas**: Sistema de animaciones (Animator) vinculado directamente al State Machine.
* **Diseño de Audio**: Implementación de efectos de sonido y diálogos grabados en cabina insonorizada para dar retroalimentación (feedback) durante el combate.

### 3. Sistemas, Redes e Infraestructura Web (DevOps/SysAdmin)
* **Arquitectura DMZ**: Configuración de una Zona Desmilitarizada para aislar los servicios públicos y proteger la red interna.
* **Servidor Web**: Despliegue en un entorno Linux (`/var/www/html`) para alojar la landing page del proyecto.
* **Seguridad**: Configuración y gestión de certificados SSL (autosignados) para conexiones seguras (HTTPS).

## 📸 Galería y Demostración

*(Añade aquí GIFs o imágenes de tu juego)*
* `![Gameplay](ruta-al-gif-animado)`
* `![Máquina de estados en Unity](ruta-a-captura-de-codigo)`
* `![Esquema DMZ](ruta-a-esquema-de-red)`

## 🛠️ Instalación y Ejecución

Para probar el proyecto en un entorno local:

1. Clona este repositorio:
   ```bash
   git clone [https://github.com/TuUsuario/ITBrawl.git](https://github.com/TuUsuario/ITBrawl.git)
