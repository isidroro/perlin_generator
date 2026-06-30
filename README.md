# PerlinNoiseGenerator

Prototipo para generar mapas de ruido Perlin, visualizarlos en WinForms y exportarlos como archivos CSV para su utilización en otras aplicaciones.

### Funcionalidad:

Ejecutar la aplicación mostrará una ventana con un mapa ya generado. Presionar la barra espaciadora generará un nuevo mapa. Cada mapa visualizado se exportará automaticamente al directorio bin/Debug/Exports.
La implementación redondea el ruido a 1 o 0, de manera que el CSV contendrá solo estos dos valores, simulando así un espacio bidimensional.

### Uso:
La aplicación solo es compatible con Windows debido a que emplea WinForms. No existen dependencias externas (FastNoiseLite está incluido en el respositorio, bajo su propia licencia MIT).
Bastará con clonar y compilar el proyecto.
