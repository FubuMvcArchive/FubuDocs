using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using FubuDocs.Topics;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuDocs.Tests.Topics
{
    [TestFixture]
    public class TopicTester
    {
        [SetUp]
        public void SetUp()
        {
            theProject = new ProjectRoot();
            a = new Topic(theProject, new StubTopicFile());
            b = new Topic(theProject, new StubTopicFile());
            c = new Topic(theProject, new StubTopicFile());
            d = new Topic(theProject, new StubTopicFile());
            e = new Topic(theProject, new StubTopicFile());
        }

        private Topic a;
        private Topic b;
        private Topic c;
        private Topic d;
        private Topic e;
        private ProjectRoot theProject;

        private void A_should_Have_B_then_C_then_D()
        {
            a.FirstChild.ShouldBeTheSameAs(b);
            a.LastChild.ShouldBeTheSameAs(d);
            a.ChildNodes.ShouldHaveTheSameElementsAs(b, c, d);

            b.PreviousSibling.ShouldBeNull();
            b.NextSibling.ShouldBeTheSameAs(c);
            b.Parent.ShouldBeTheSameAs(a);

            c.PreviousSibling.ShouldBeTheSameAs(b);
            c.NextSibling.ShouldBeTheSameAs(d);
            b.Parent.ShouldBeTheSameAs(a);

            d.PreviousSibling.ShouldBeTheSameAs(c);
            d.NextSibling.ShouldBeNull();
            b.Parent.ShouldBeTheSameAs(a);
        }

        [Test]
        public void replace_with_parent_and_next()
        {
            a.AppendChild(b);
            a.AppendChild(c);

            b.ReplaceWith(d);

            a.FirstChild.ShouldBeTheSameAs(d);
            d.NextSibling.ShouldBeTheSameAs(c);
            c.PreviousSibling.ShouldEqual(d);
        }

        [Test]
        public void replace_with_parent_and_previous_and_next()
        {
            a.AppendChild(b);
            a.AppendChild(c);
            a.AppendChild(d);

            c.ReplaceWith(e);

            b.NextSibling.ShouldBeTheSameAs(e);
            e.NextSibling.ShouldBeTheSameAs(d);
            d.PreviousSibling.ShouldBeTheSameAs(e);
        }

        [Test]
        public void replace_with_parent_and_previous()
        {
            a.AppendChild(b);
            a.AppendChild(c);

            c.ReplaceWith(d);

            b.NextSibling.ShouldBeTheSameAs(d);
            d.PreviousSibling.ShouldBeTheSameAs(b);
            d.NextSibling.ShouldBeNull();
        }

        [Test]
        public void append_multiple_children()
        {
            a.AppendChild(b);
            a.AppendChild(c);
            a.AppendChild(d);

            A_should_Have_B_then_C_then_D();
        }

        [Test]
        public void append_one_child()
        {
            a.AppendChild(b);

            a.FirstChild.ShouldBeTheSameAs(b);
            a.LastChild.ShouldBeTheSameAs(b);
            a.ChildNodes.ShouldHaveTheSameElementsAs(b);

            b.Parent.ShouldBeTheSameAs(a);
            b.NextSibling.ShouldBeNull();
            b.PreviousSibling.ShouldBeNull();
        }

        [Test]
        public void find_Next_with_children()
        {
            a.AppendChild(b);
            a.InsertAfter(c);

            a.FindNext().ShouldEqual(b);
        }

        [Test]
        public void find_index_walks_up_top()
        {
            a.AppendChild(b);
            b.AppendChild(c);
            c.AppendChild(d);
            d.AppendChild(e);

            e.FindIndex().ShouldEqual(a);
            d.FindIndex().ShouldEqual(a);
            c.FindIndex().ShouldEqual(a);
            b.FindIndex().ShouldEqual(a);

            a.FindIndex().ShouldBeNull();
        }

        [Test]
        public void find_next_going_up_one_level_to_do_so()
        {
            a.AppendChild(b);
            a.InsertAfter(c);

            b.FindNext().ShouldEqual(c);
        }

        [Test]
        public void find_next_going_up_two_levels_to_do_so()
        {
            a.AppendChild(b);
            b.AppendChild(c);
            a.InsertAfter(d);

            c.FindNext().ShouldEqual(d);
        }

        [Test]
        public void find_next_with_sibling()
        {
            a.InsertAfter(b);
            a.FindNext().ShouldEqual(b);
        }

        [Test]
        public void find_previous_going_to_the_parent()
        {
            a.AppendChild(b);
            b.FindPrevious().ShouldEqual(a);
        }

        [Test]
        public void find_previous_with_no_parent_or_sibling()
        {
            a.FindPrevious().ShouldBeNull();
            a.AppendChild(b);
            a.FindPrevious().ShouldBeNull();
        }

        [Test]
        public void find_previous_with_sibling()
        {
            a.InsertAfter(b);
            b.FindPrevious().ShouldEqual(a);
        }

        [Test]
        public void initial_state_nulls()
        {
            a.NextSibling.ShouldBeNull();
            a.Parent.ShouldBeNull();
            a.PreviousSibling.ShouldBeNull();
            a.LastChild.ShouldBeNull();

            a.ChildNodes.Any().ShouldBeFalse();
        }

        [Test]
        public void insert_before_as_the_first_child()
        {
            a.AppendChild(c);
            a.AppendChild(d);

            c.InsertBefore(b);

            A_should_Have_B_then_C_then_D();
        }

        [Test]
        public void insert_before_in_the_middle_with_a_parent()
        {
            a.AppendChild(b);
            a.AppendChild(d);

            d.InsertBefore(c);

            A_should_Have_B_then_C_then_D();
        }

        [Test]
        public void insert_before_with_no_parent()
        {
            b.InsertBefore(c);

            b.PreviousSibling.ShouldBeTheSameAs(c);
            c.NextSibling.ShouldBeTheSameAs(b);

            b.NextSibling.ShouldBeNull();
            c.PreviousSibling.ShouldBeNull();

            b.Parent.ShouldBeNull();
            c.Parent.ShouldBeNull();
        }

        [Test]
        public void prepend_child()
        {
            a.AppendChild(c);
            a.AppendChild(d);

            a.PrependChild(b);

            A_should_Have_B_then_C_then_D();
        }

        [Test]
        public void prepend_one_child()
        {
            a.PrependChild(b);

            a.FirstChild.ShouldBeTheSameAs(b);
            a.LastChild.ShouldBeTheSameAs(b);
            a.ChildNodes.ShouldHaveTheSameElementsAs(b);

            b.Parent.ShouldBeTheSameAs(a);
            b.NextSibling.ShouldBeNull();
            b.PreviousSibling.ShouldBeNull();
        }

        [Test]
        public void regex_fun()
        {
            //var regex = @"<!--(?!\[).*?(?!<\])-->";
            string regex = @"<!--(.*?)-->";

            MatchCollection matches = Regex.Matches("<!-- something -->", regex);
            // MatchCollection matches = Regex.Matches(template, @"\{(\w+)\}");

            foreach (Match match in matches)
            {
                Debug.WriteLine(match);
                string key = match.Groups[1].Value;

                Debug.WriteLine(key);
            }
        }

        [Test]
        public void remove_from_end_of_stack()
        {
            a.AppendChild(b);
            a.AppendChild(c);
            a.AppendChild(d);
            a.AppendChild(e);

            e.Remove();

            A_should_Have_B_then_C_then_D();
        }

        [Test]
        public void remove_from_middle()
        {
            a.AppendChild(b);
            a.AppendChild(e);
            a.AppendChild(c);
            a.AppendChild(d);


            e.Remove();

            A_should_Have_B_then_C_then_D();
        }

        [Test]
        public void remove_from_top()
        {
            a.AppendChild(e);
            a.AppendChild(b);
            a.AppendChild(c);
            a.AppendChild(d);


            e.Remove();

            A_should_Have_B_then_C_then_D();
        }

        [Test]
        public void remove_only_child()
        {
            a.AppendChild(e);
            e.Remove();

            initial_state_nulls();
        }

        [Test]
        public void there_is_no_next()
        {
            a.AppendChild(b);
            b.AppendChild(c);

            c.FindNext().ShouldBeNull();
        }

        [Test]
        public void compare_to()
        {
            Topic.CompareName("a", "b").ShouldEqual(-1);
            Topic.CompareName("b", "a").ShouldEqual(1);
            Topic.CompareName("a", "a").ShouldEqual(0);

            Topic.CompareName("1.b", "2.a").ShouldEqual(-1);
            Topic.CompareName("2.b", "1.a").ShouldEqual(1);

            Topic.CompareName("10.a", "1.b").ShouldEqual(1);
            Topic.CompareName("1.b", "10.a").ShouldEqual(-1);
        }
    }
}