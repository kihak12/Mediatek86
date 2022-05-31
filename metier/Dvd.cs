using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediatek86.metier
{
    public class Dvd : LivreDvd
    {

        private readonly int duree;
        private readonly string realisateur;
        private readonly string synopsis;
        private readonly string url;

        public Dvd(string id, string titre, string image, int duree, string realisateur, string synopsis,
            string idGenre, string genre, string idPublic, string lePublic, string idRayon, string rayon, string url)
            : base(id, titre, image, idGenre, genre, idPublic, lePublic, idRayon, rayon)
        {
            this.duree = duree;
            this.realisateur = realisateur;
            this.synopsis = synopsis;
            this.url = url;
        }

        public int Duree { get => duree; }
        public string Realisateur { get => realisateur; }
        public string Synopsis { get => synopsis; }
        public string Url { get => url; }

    }
}
