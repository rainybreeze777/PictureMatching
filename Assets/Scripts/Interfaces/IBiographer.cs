using System;
using System.Collections.Generic;

public interface IBiographer {

    void Visit(int destId);
    void Leave();
    bool AlreadyVisitedFromCurrentPoint(int destId);
    void InitFromGameSave(GameSave save);
    bool IsAtMap();
    
}
