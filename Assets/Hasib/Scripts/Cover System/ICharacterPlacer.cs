using UnityEngine;

public interface ICharacterPlacer 
{
    

    bool cover { get; set; }
    void BeginMoveToCover(Vector3 targetPos);
}
