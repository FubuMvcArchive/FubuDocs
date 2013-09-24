using FubuCore;

namespace FubuDocs.Tools
{
    public class NewTopic : IDelta
    {
        private readonly TopicToken _token;

        public NewTopic(TopicToken token)
        {
            _token = token;
        }

        public void Prepare()
        {
        }

        public void Execute()
        {
            TopicToken.FileSystem.AlterFlatFile(_token.File, list =>
            {
                list.Add("<!--Title: {0}-->".ToFormat(_token.Title));

                if (_token.Url.IsNotEmpty())
                {
                    list.Add("<!--Url: {0}-->".ToFormat(_token.Url.ToLower()));
                }

                list.Add("");
                list.Add("<markdown>");
                list.Add("TODO(Write some content!)");
                list.Add("</markdown>");
                list.Add("");
            });
        }

        protected bool Equals(NewTopic other)
        {
            return Equals(_token, other._token);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((NewTopic) obj);
        }

        public override int GetHashCode()
        {
            return (_token != null ? _token.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return string.Format("Create topic {0}", _token);
        }
    }
}