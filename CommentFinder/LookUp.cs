using System;
using System.Collections.Generic;
using System.Text;

namespace CommentFinder
{
    /**
     * The LookUp class is used to store how difference language comment their code
     * Instead of using a database, we will be using a JSON file to store the languages
     * and their commenting styles
     */
    class LookUp
    {
        public string FileExtension { get; set; }
        public string SingleLineComment { get; set; }
        public string MultilineCommentStart { get; set; }
        public string MultilineCommentMiddle { get; set; }
        public string MultilineCommentEnd { get; set; }
    }
}
