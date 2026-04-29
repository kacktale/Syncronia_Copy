using System.Collections.Generic;
using UnityEngine;

public class SongCarouselManager : MonoBehaviour
{
    [SerializeField] private List<SongData> originSongDB;
    public List<SongData> ReadOnlySongList => originSongDB;
}
