This is a simple simulation, every object is a cell on simulation field. 

[![](https://img.youtube.com/vi/s6dcKFjWTJE/0.jpg)](https://www.youtube.com/watch?v=s6dcKFjWTJE)

## Types:
Red static cells are food, that bugs eat.<br>
Gray static cells are spikes, bugs die when stands on them.<br>
Moving green (or not green) cells are bugs, every division their color some changes (the more bug's neural network mutates, the more color changes).<br>

## How does it works:
Neural network sees 24 near cells (neighbors and their neighbors), knows self health and energy, then some magic and boom - it returns direction, where bug will move!
