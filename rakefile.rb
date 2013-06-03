require 'bundler/setup'
require 'fuburake'

@solution = FubuRake::Solution.new do |sln|
	sln.compile = {
		:solutionfile => 'src/FubuDocs.sln'
	}

	sln.assembly_info = {
		:product_name => "FubuDocs",
		:copyright => 'Copyright 2008-2013 Jeremy D. Miller, et al. All rights reserved.'
	}

	sln.ripple_enabled = true
	sln.fubudocs_enabled = true
	
	sln.ci_steps = ["gem:archive"]
    sln.assembly_bottle 'FubuDocs'
end

BUILD_NUMBER = @solution.options[:build_number]

load File.expand_path('../fubudocs/Rakefile', __FILE__)
