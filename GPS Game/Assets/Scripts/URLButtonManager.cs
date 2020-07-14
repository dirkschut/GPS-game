using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class URLButtonManager : MonoBehaviour
{
    /// <summary>
    /// Opens the given URL.
    /// </summary>
    /// <param name="URL">THe URL to open</param>
    public void OnClick(string URL)
    {
        Application.OpenURL(URL);
    }
}
