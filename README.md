<h1 align="center">Panel Cutting</h1>

# Задача
## Дано
Профиль фасада здания - многоугольник, заданный координатами вершин, с общей высотой до 13,5 м.
Панели на фасаде устанавливаются вертикально.
Фасадная панель отрезается так, чтобы в месте установки полностью закрыть фасад, но быть минимальной длины.
Фасадные панели вырезаются из исходных панелей шириной 0,5 м и длиной до 13,5 м.

## Нужно
Вычислить количество и длины необходимых фасадных панелей.

## Пример входных данных
```
{
  "Profile": [
   {"X": 0, "Y": 0},
   {"X": -1703, "Y": 5838},
   {"X": 5949, "Y": 9964},
   {"X": 10494, "Y": 7549},
   {"X": 10494, "Y": 339}]
}
```
Точки соединены последовательно, тоесть i-ая с (i+1)-ой и N-ая с 1-ой.

# Решение
## Алгоритм
Для каждой пары соединенных точек создадим отрезок. Создадим дерево интервалов в которое запишем все отрезке в качестве интервалов по оси X.<br/>
Найдем самую левую точку профиля. Начиная с неё будем откладывать интервалы по оси X с шириной панели. Для каждого такого интервала найдем все пересечения с помощью дерева интервалов.<br/>
Найдем минимальную и максимальную точки на оси Y для всех пересеченных отрезков в данном интервале. Разницей этих значений будет длина панели.

## Алгоритмическая сложность решения - ```O(L * log(N) + N)```
N - количество точек в профиле фасада<br/>
L - длина фасада<br/>
Так как для каждого интервала идёт обход дерева интервалов получаем левую часть суммы - ```L * log(N)```<br/>
Но так же мы обработаем все отрезки пройдя по профилю, даже если интервал будет всего один то все N отрезков будут обработаны, из этого получаем правую часть суммы - ```N```

## Описание реализации
Репозиторий содержит в себе 3 проекта:
* PanelCutting.API - пример реализации приложения для использования библиотеки
* PanelCutting.Tests - проект с юнит-тестами решения
* PanelCutting - библиотека с решением

## Использование
Библиотека предоставляет всего 4 публичных класса:
* Point - сущность точки в пространстве
* Profile - сущность данных о профиле (набор точек полигона)
* ProfileCutter - сущность отвечающая за раскрой профиля на панели
* PanelCut - сущность данных о разрезе панели (длина)

Для начала нужно создать объект типа ProfileCutter с необходимой шириной раскроя панелей.<br/>
Затем создать объект типа Profile с необходимой формой профиля, используя объекты типа Point.<br/>
Результат получается при передаче объекта Profile в функцию Cut() у ProfileCutter.<br/>
Результат имеет тип IEnumerable\<PanelCut\>.

## Упрощения
* Нет валидации высоты фасада, так же нет ограничения для длин вырезаемых панелей. При указаных входных данных это ничего не меняет, но при изменении условий это нужно будет добавлять в код.
* Нет полной валидации фасада, под это только выделено место.
