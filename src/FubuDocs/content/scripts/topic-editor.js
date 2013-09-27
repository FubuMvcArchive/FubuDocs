$(document).ready(function () {
    $('#topic-tree').nestable({ group: 1 });
    $('#new-leaf:first-child').nestable({ group: 1 });

    $('#topics-tab').click();

    var adderView = new TopicAdderView();
    var controller = new TopicController(adderView);


    $('#topic-tree').on('click', 'li.dd-item', function (e) {
        var leaf = new TopicLeaf(this);
        controller.select(leaf);
        e.stopPropogation();
    });

    $('#add-topic-button').click(function() {
        controller.addTopic();
    });


});


function TopicAdderView() {
    var self = this;

    $('#topic-editing-content').hide();

    var form = $('#add-topic-form');

    var existingEditor = new EditorPane($('#inline-editor'));
    $('input', existingEditor.form).change(function() {
        $('.input-append a', existingEditor.form).show();
    });

    $('#change-current-topic').click(function() {
        existingEditor.commit();
        $('.input-append a', existingEditor.form).hide();
    });


    $('#reset-current-topic').click(function () {
        existingEditor.reset();
        $('.input-append a', existingEditor.form).hide();
    });
    


    var editor = new EditorPane(form);

    self.activate = function(leaf) {
        $('#topic-editing-content').show();
        $('#inline-editor').show();
        existingEditor.edit(leaf);

        $('.input-append a', existingEditor.form).hide();

        $('.parent-title').html(leaf.get('title'));

        editor.clear();
    };

    return self;
}

function TopicController(adder) {
    var self = this;

    self.editor = null;

    self.select = function (leaf) {
        if (self.current != null) {
            self.current.markInactive();
        }

        self.current = leaf;
        leaf.markActive();
        
        adder.activate(leaf);
        

    };

    self.addTopic = function() {
        alert("we're adding a topic now");
    };

    return self;
}

function EditorPane(form) {
    var self = this;
    self.form = form;

    self.edit = function (leaf) {
        self.leaf = leaf;
        self.reset();
    };

    self.push = function(prop) {
        var val = $('.' + prop, form).val();
        self.leaf.set(prop, val);
    };

    self.pull = function(prop) {
        var val = self.leaf.get(prop);
        $('.' + prop, form).val(val);
    };

    self.commit = function() {
        self.push('title');
        self.push('url');
        self.push('key');
    };

    self.reset = function() {
        self.pull('title');
        self.pull('url');
        self.pull('key');
    };

    self.clear = function () {
        $('.title', form).val('').focus();
        $('.url', form).val('');
        $('.key', form).val('');
        $('.sections', form).val('');
    };

    return self;
}

// Wraps the <li> for a single item
function TopicLeaf(item) {
    var self = this;
    
    if (item == null) {
        // TODO -- go find the template and clone it.    
    }

    self.item = item;

    self.get = function(key) {
        return $(item).data(key);
    };

    self.set = function(key, value) {
        $(item).data(key, value);
        
        if (key == 'title') {
            $('.topic-title', item).html(value);
        }
    };

    self.markActive = function() {
        $(item).addClass('active-topic');
    };

    self.markInactive = function() {
        $(item).removeClass('active-topic');
    };

    return self;
}


/*
<li class="dd-item dd3-item" data-key="index" data-url="index" data-title="Index" data-id="&quot;a9f68b50-35ef-4fe6-8d56-eac4f42bba31&quot;">
  <button type="button" data-action="collapse">Collapse</button>
  <button type="button" data-action="expand" style="display: none;">Expand</button>
  <div class="dd-handle dd3-handle"></div>
  <div class="topic-title dd3-content">Index</div>
  <ol class="dd-list">
    <li class="dd-item dd3-item" data-key="starting" data-url="starting" data-title="Starting" data-id="&quot;5af9124c-b54a-45a7-b392-bdafb4d39a44&quot;"><button type="button" data-action="collapse">Collapse</button><button type="button" data-action="expand" style="display: none;">Expand</button><div class="dd-handle dd3-handle"></div><div class="topic-title dd3-content">Starting</div><ol class="dd-list"></ol></li><li class="dd-item dd3-item" data-key="splashpages" data-url="splashpages" data-title="Splash Pages" data-id="&quot;822845b7-44c5-44a1-8f82-aea8da721ef3&quot;"><button type="button" data-action="collapse">Collapse</button><button type="button" data-action="expand" style="display: none;">Expand</button><div class="dd-handle dd3-handle"></div><div class="topic-title dd3-content">Splash Pages</div><ol class="dd-list"></ol></li><li class="dd-item dd3-item" data-key="topics" data-url="topics" data-title="Topics" data-id="&quot;f6fc9df4-3850-4517-9eec-ef2df3e1ab29&quot;"><button type="button" data-action="collapse">Collapse</button><button type="button" data-action="expand" style="display: none;">Expand</button><div class="dd-handle dd3-handle"></div><div class="topic-title dd3-content">Topics</div><ol class="dd-list"></ol></li><li class="dd-item dd3-item" data-key="tools" data-url="tools" data-title="Authoring Tools" data-id="&quot;50ba675f-b619-4b7b-897b-6dac9bdcea28&quot;"><button type="button" data-action="collapse">Collapse</button><button type="button" data-action="expand" style="display: none;">Expand</button><div class="dd-handle dd3-handle"></div><div class="topic-title dd3-content">Authoring Tools</div><ol class="dd-list"></ol></li><li class="dd-item dd3-item" data-key="samples" data-url="samples" data-title="Samples" data-id="&quot;3958f4b9-8025-4e5d-9f7b-5f1da4b9e810&quot;"><button type="button" data-action="collapse">Collapse</button><button type="button" data-action="expand" style="display: none;">Expand</button><div class="dd-handle dd3-handle"></div><div class="topic-title dd3-content">Samples</div><ol class="dd-list"></ol></li><li class="dd-item dd3-item" data-key="codesnippets" data-url="codesnippets" data-title="Code Snippets" data-id="&quot;c39c373b-7033-45e7-9a43-209f88d60fcb&quot;"><button type="button" data-action="collapse">Collapse</button><button type="button" data-action="expand" style="display: none;">Expand</button><div class="dd-handle dd3-handle"></div><div class="topic-title dd3-content">Code Snippets</div><ol class="dd-list"></ol></li><li class="dd-item dd3-item" data-key="viewhelpers" data-url="viewhelpers" data-title="Topic View Helpers" data-id="&quot;90e38afc-d89f-4fb1-8e8e-6fbd89b16f74&quot;"><button type="button" data-action="collapse">Collapse</button><button type="button" data-action="expand" style="display: none;">Expand</button><div class="dd-handle dd3-handle"></div><div class="topic-title dd3-content">Topic View Helpers</div><ol class="dd-list"></ol></li><li class="dd-item dd3-item" data-key="cli" data-url="cli" data-title="Command Line Documentation" data-id="&quot;218c7c3d-3dec-44a4-b5c9-e9e0873b9cbf&quot;"><button type="button" data-action="collapse">Collapse</button><button type="button" data-action="expand" style="display: none;">Expand</button><div class="dd-handle dd3-handle"></div><div class="topic-title dd3-content">Command Line Documentation</div><ol class="dd-list"></ol></li><li class="dd-item dd3-item" data-key="index" data-url="commands" data-title="FubuDocs at the command line" data-id="&quot;2310b716-76e1-463c-899a-30da7f6778b0&quot;"><button type="button" data-action="collapse">Collapse</button><button type="button" data-action="expand" style="display: none;">Expand</button><div class="dd-handle dd3-handle"></div><div class="topic-title dd3-content">FubuDocs at the command line</div><ol class="dd-list"><li class="dd-item dd3-item" data-key="snippets" data-url="commands/snippets" data-title="Gathering Code Snippets" data-id="&quot;a95a6b20-121e-43a4-8a80-acbaddd6ce57&quot;"><button type="button" data-action="collapse">Collapse</button><button type="button" data-action="expand" style="display: none;">Expand</button><div class="dd-handle dd3-handle"></div><div class="topic-title dd3-content">Gathering Code Snippets</div><ol class="dd-list"></ol></li><li class="dd-item dd3-item" data-key="bottling" data-url="commands/bottling" data-title="Bottling the FubuDocs Projects" data-id="&quot;2fe8138f-feca-4f1a-b9d3-edb5127bbbb8&quot;"><button type="button" data-action="collapse">Collapse</button><button type="button" data-action="expand" style="display: none;">Expand</button><div class="dd-handle dd3-handle"></div><div class="topic-title dd3-content">Bottling the FubuDocs Projects</div><ol class="dd-list"></ol></li><li class="dd-item dd3-item" data-key="running" data-url="commands/running" data-title="Running your FubuDocs Project" data-id="&quot;73d20800-b43c-4178-90dd-0cbcc4d60a49&quot;"><button type="button" data-action="collapse">Collapse</button><button type="button" data-action="expand" style="display: none;">Expand</button><div class="dd-handle dd3-handle"></div><div class="topic-title dd3-content">Running your FubuDocs Project</div><ol class="dd-list"></ol></li><li class="dd-item dd3-item" data-key="smoke" data-url="commands/smoke" data-title="Smoke Testing a Documentation Project" data-id="&quot;d877da9d-ca28-4565-92d4-2289148b9f7c&quot;"><button type="button" data-action="collapse">Collapse</button><button type="button" data-action="expand" style="display: none;">Expand</button><div class="dd-handle dd3-handle"></div><div class="topic-title dd3-content">Smoke Testing a Documentation Project</div><ol class="dd-list"></ol></li><li class="dd-item dd3-item" data-key="exporting" data-url="commands/exporting" data-title="Exporting to Static Html" data-id="&quot;b9c14364-6e04-43d2-8366-e570753fc8dd&quot;"><button type="button" data-action="collapse">Collapse</button><button type="button" data-action="expand" style="display: none;">Expand</button><div class="dd-handle dd3-handle"></div><div class="topic-title dd3-content">Exporting to Static Html</div><ol class="dd-list"></ol></li><li class="dd-item dd3-item" data-key="list" data-url="commands/list" data-title="List Topics" data-id="&quot;8fab9d3c-e0b3-4bd1-aa42-acc32bba55a4&quot;"><button type="button" data-action="collapse">Collapse</button><button type="button" data-action="expand" style="display: none;">Expand</button><div class="dd-handle dd3-handle"></div><div class="topic-title dd3-content">List Topics</div><ol class="dd-list"></ol></li><li class="dd-item dd3-item" data-key="addtopics" data-url="commands/addtopics" data-title="Add Topics in a Batch" data-id="&quot;aaa1fcf7-c672-4183-9e60-545cd9dfa506&quot;"><button type="button" data-action="collapse">Collapse</button><button type="button" data-action="expand" style="display: none;">Expand</button><div class="dd-handle dd3-handle"></div><div class="topic-title dd3-content">Add Topics in a Batch</div><ol class="dd-list"></ol></li><li class="dd-item dd3-item" data-key="reordering" data-url="commands/reordering" data-title="Reordering Topics within a Folder" data-id="&quot;e1903e7d-ea25-454a-a99d-614d2493f62a&quot;"><button type="button" data-action="collapse">Collapse</button><button type="button" data-action="expand" style="display: none;">Expand</button><div class="dd-handle dd3-handle"></div><div class="topic-title dd3-content">Reordering Topics within a Folder</div><ol class="dd-list"></ol></li></ol></li></ol></li>
*/
