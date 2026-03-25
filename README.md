# 🥊 ITBrawl - Unity 3D Brawler

[![Unity](https://img.shields.io/badge/Unity-100000?style=for-the-badge&logo=unity&logoColor=white)](https://unity.com/)
[![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/csharp/)

> Un videojuego de lucha 3D desarrollado en Unity que implementa una arquitectura robusta basada en **Máquinas de Estados (State Machines)** para el control de personajes.

## 🚀 Sobre el Proyecto

**ITBrawl** es un proyecto desarrollado por el equipo InTerBits. El objetivo principal ha sido crear un brawler (juego de lucha) funcional desde cero, abarcando las fases clave del desarrollo de un videojuego: desde el modelado 3D y la integración de audio personalizado, hasta la programación de físicas, combates y estados de los personajes.

Este repositorio contiene el código fuente íntegro del proyecto en Unity.

## 💻 Arquitectura y Habilidades Técnicas

Este código demuestra la aplicación de buenas prácticas de programación y conocimientos avanzados en el ecosistema de Unity:

### 1. Programación Orientada a Objetos y Patrones de Diseño (C#)
El control de los personajes se ha construido utilizando el **Patrón de Diseño State Machine**. Esto garantiza un código limpio, escalable y mantenible, evitando las tradicionales y propensas a errores cadenas de `if/else` en el `Update`.
* **`PlayerStateManager.cs`**: Gestor principal (Contexto) que controla las transiciones entre estados.
* **Estados modulares**: Implementación de clases independientes y desacopladas para cada comportamiento del jugador (`IdleState.cs`, `WalkingState.cs`, `JumpState.cs`, `AirState.cs`, `AttackingState.cs`).
* **Sistema de Combate**: Gestión precisa de hitboxes y cálculo de daño a través del script `CharacterAttacks.cs`.

### 2. Desarrollo e Integración en Unity
* **Integración 3D**: Uso de modelos 3D y creación de entornos personalizados (Mapa Nuvulet).
* **Sistemas de Animación**: Uso del componente `Animator` y sincronización directa de las animaciones con la Máquina de Estados por código.
* **Diseño y Gestión de Audio**: Implementación de `AudioSources` para efectos de sonido dinámicos y diálogos grabados en estudio, diseñados para ofrecer *game feel* y retroalimentación al jugador durante los combates.

## 📜 Descripción de Scripts Principales

A continuación se detallan los scripts fundamentales que componen el núcleo del juego:

### ⚙️ Gestor Principal y Plantillas
* **`PlayerStateManager.cs`**: Script principal (Contexto) de la máquina de estados. Lee los *inputs* del jugador y el entorno (ej. detectar el suelo mediante RayCast) para decidir el estado actual del personaje y ejecutar transiciones.
* **`PlayerState.cs`**: Clase base de la que heredan todos los estados. Define las funciones obligatorias que deben implementar (`Enter()`, `Exit()`, `Update()`, `FixedUpdate()`).
* **`CharacterAttacks.cs`**: Clase abstracta/plantilla que define las funciones de ataque. Permite aplicar polimorfismo para que cada luchador tenga su propia lógica de daño.

### 🚶‍♂️ Estados del Jugador (State Machine)
* **`WalkingState.cs`**: Lógica de movimiento en el suelo. Aplica fuerzas según el *joystick*, controla la aceleración, limita la velocidad máxima y gestiona los giros.
* **`AirState.cs`**: Control aéreo. Gestiona la lógica del doble salto y añade gravedad extra para mejorar la sensación de las caídas.
* **`JumpState.cs`**: Aplica un impulso vertical al entrar en el estado para realizar el salto.
* **`IdleState.cs`**: Estado de reposo. Su función principal es desacelerar la velocidad del jugador hasta que se detiene por completo.
* **`AttackingState.cs`**: Ejecuta los ataques. Lee la dirección del *joystick* y llama a la función de ataque correspondiente en el script del personaje.
* **`StunnedState.cs`**: Estado de aturdimiento. Retira el control al jugador al recibir daño, reproduce la animación de impacto y devuelve al jugador al estado *Idle* tras una corrutina.

### ⚔️ Lógica de Personajes y Vida
* **`PlayerLive.cs`**: Sistema de salud del jugador. Gestiona la recepción de daño (`TakeDamage()`), curación (`Heal()`), actualización de la interfaz de usuario (UI) y la muerte del personaje (`Die()`). También aplica retroceso (knockback) al recibir un golpe.
* **`MartiAttacks.cs` / `XaviAttacks.cs` / `DaniAttacks.cs`**: Scripts que heredan de `CharacterAttacks.cs`. Contienen la lógica específica, efectos y *hitboxes* de los ataques únicos de cada personaje.

## 📸 Galería y Demostración

* `![Gameplay](ruta-al-gif-animado)`
* `![Estructura del State Machine](ruta-a-captura-de-codigo-limpio)`

## 🛠️ Instalación y Ejecución

Para explorar el código o probar el proyecto en un entorno local:

1. Clona este repositorio:
   ```bash
   git clone https://github.com/ElCire12/ITBrawl.git
   ```
2. Abre el proyecto utilizando **Unity Hub** (Versión de Unity`2022.3.46f1`).
3. Navega a la carpeta `Scenes` y abre la escena principal (`MainScene.unity`).
4. Pulsa **Play** en el editor para iniciar el juego.

## 👨‍💻 Autores

**InTerBits Team**
* **Èric Moral Pereira** - *Programación en C# / Unity* - [LinkedIn](https://www.linkedin.com/in/eric-moral-pereira) | [GitHub](https://github.com/ElCire12/)
* Albert Domingo Montemar
* Martí Del Valle Gonzalez
