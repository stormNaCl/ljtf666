using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IModule
{
    public void Initial();
    public void Update();
    public void Handle(object msg);
}
