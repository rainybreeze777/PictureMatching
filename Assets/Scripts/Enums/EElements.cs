public enum EElements {
    NONE, METAL, WOOD, WATER, FIRE, EARTH
}

public static class EElementsExtensions {
    public static EElements Destroys(this EElements e) {
        switch(e) {
            case EElements.METAL:
                return EElements.WOOD;
            case EElements.WOOD:
                return EElements.EARTH;
            case EElements.WATER:
                return EElements.FIRE;
            case EElements.FIRE:
                return EElements.METAL;
            case EElements.EARTH:
                return EElements.WATER;
            default:
                return EElements.NONE;
        }
    }

    public static EElements DestroyedBy(this EElements e) {
        switch(e) {
            case EElements.METAL:
                return EElements.FIRE;
            case EElements.WOOD:
                return EElements.METAL;
            case EElements.WATER:
                return EElements.EARTH;
            case EElements.FIRE:
                return EElements.WATER;
            case EElements.EARTH:
                return EElements.WOOD;
            default:
                return EElements.NONE;
        }
    }

    public static EElements Produces(this EElements e) {
        switch(e) {
            case EElements.METAL:
                return EElements.WATER;
            case EElements.WOOD:
                return EElements.FIRE;
            case EElements.WATER:
                return EElements.WOOD;
            case EElements.FIRE:
                return EElements.EARTH;
            case EElements.EARTH:
                return EElements.METAL;
            default:
                return EElements.NONE;
        }
    }

    public static EElements ProducedBy(this EElements e) {
        switch(e) {
            case EElements.METAL:
                return EElements.EARTH;
            case EElements.WOOD:
                return EElements.WATER;
            case EElements.WATER:
                return EElements.METAL;
            case EElements.FIRE:
                return EElements.WOOD;
            case EElements.EARTH:
                return EElements.FIRE;
            default:
                return EElements.NONE;
        }
    }
}