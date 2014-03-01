# FubuDocs

FubuDocs is a tool that is built on top of FubuMVC and other members of the
Fubu family of frameworks. It is designed to help you keep your documentation
right alongside your actual code so that *nothing* gets out of sync.

To make hosting and deployment *simple*, it exports all of the rendered
documentation into plain ol' static html. You get the power of FubuMVC with the
simplicity of Github pages (or anywhere else you want to host your documentation).

Building the Source
-------------------
1. Clone the repository: `git clone https://github.com/DarthFubuMVC/FubuDocs.git`
2. Make sure you have [ruby installed][ruby] (>= 1.9.3)
3. If you are installing Ruby for the first time, install [Bundler][bundler] with `gem install bundler` at a command prompt
4. In the root, run `rake` (What is [rake][rake]?)
5. Open `FubuDocs.sln` in VS2010 (or run `rake sln` from the root)

[ruby]: http://www.ruby-lang.org/en/downloads/
[bundler]: http://bundler.io/
[rake]: http://rake.rubyforge.org/
