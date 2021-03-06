require 'bundler/setup'
require 'fuburake'

@solution = FubuRake::Solution.new do |sln|
	sln.compile = {
		:solutionfile => 'src/FubuDocs.sln'
	}
	
	sln.clean = ['src/FubuDocsRunner/bin/Debug/fubu-content', 'src/Host/fubu-content']
	
	sln.assembly_info = {
		:product_name => "FubuDocs",
		:copyright => 'Copyright 2008-2013 Jeremy D. Miller, et al. All rights reserved.'
	}

	sln.ripple_enabled = true
	sln.fubudocs_enabled = true
	
	sln.ci_steps = ["gem:archive"]
    sln.assembly_bottle 'FubuDocs'
	
	# TODO -- add this later:  , :include_in_ci => true
	sln.export_docs({:repository => 'git@github.com:DarthFubuMVC/FubuDocs.git', :include => 'FubuDocs'})
end

BUILD_NUMBER = @solution.options[:build_number]

load File.expand_path('../fubudocs/Rakefile', __FILE__)


task :open => [:compile] do
  sh 'src/FubuDocsRunner/bin/Debug/FubuDocsRunner.exe run --open'
end

desc "Outputs the command line usage"
task :dump_usages => [:compile] do
  sh 'src/FubuDocsRunner/bin/Debug/FubuDocsRunner.exe dump-usages fubudocs src/FubuDocs.Docs/fubudocs.cli.xml'
end
