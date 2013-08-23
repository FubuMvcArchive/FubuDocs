using System;
using System.Collections.Generic;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Assets.Http;
using FubuMVC.Core.Http;

namespace FubuDocsRunner.Exports
{
    public class ExportContentWriter : IContentWriter
    {
        private readonly ContentWriter _writer;
        private readonly IAccessedCache _cache;
        private readonly ICurrentHttpRequest _request;

        public ExportContentWriter(ContentWriter writer, IAccessedCache cache, ICurrentHttpRequest request)
        {
            _writer = writer;
            _cache = cache;
            _request = request;
        }

        public bool Write(AssetPath asset, Action<IEnumerable<AssetFile>> writeHeaders)
        {
            _cache.Enqueue(_request.RelativeUrl());
            return _writer.Write(asset, writeHeaders);
        }
    }
}