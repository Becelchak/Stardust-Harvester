using System;

public interface IBuildable
{
    /// <summary>Вызывается при размещении объекта на клетке</summary>
    void OnBuild(BuildCell cell);

    /// <summary>Вызывается при разрушении объекта</summary>
    void OnDestroyed();

    /// <summary>Стоимость постройки</summary>
    int BuildCost { get; }

    /// <summary>Доступен ли для строительства</summary>
    bool CanBuild { get; }

    /// <summary>Ссылка на клетку, на которой построен</summary>
    BuildCell BuildCell { get; set; }


    event Action<IBuildable> OnBuildDamaged;
    event Action<IBuildable> OnBuildDestroyed;
    static event Action<IBuildable> OnAnyBuildDestroyed;
}