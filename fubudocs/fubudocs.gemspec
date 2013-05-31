# coding: utf-8
Gem::Specification.new do |s|
  s.platform    = Gem::Platform::RUBY
  s.name        = 'fubudocs'
  s.version     = BUILD_NUMBER
  s.files = Dir['bin/**/*']
  s.bindir = 'bin'
  s.executables << 'fubudocs'
  s.license = "Apache 2"
  
  s.summary     = 'fubudocs runner'
  s.description = 'FubuDocs is a tool for generating project documentation using the FubuMVC framework'
  
  s.authors           = ['Jeremy D. Miller', 'Josh Arnold']
  s.email             = 'fubumvc-devel@googlegroups.com'
  s.homepage          = 'http://fubu-project.org'
  s.rubyforge_project = 'fubudocs'
end