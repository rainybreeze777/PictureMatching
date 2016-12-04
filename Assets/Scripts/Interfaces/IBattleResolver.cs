using UnityEngine;
using System.Collections.Generic;

public interface IBattleResolver {

    List<EOneExchangeWinner> ForecastExchangeResult(int forecastSize);

    void ResolveNextMove();

    void Reset();

}
