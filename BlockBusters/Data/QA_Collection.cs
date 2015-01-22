#region Prerequisites

using System;
using System.Collections.Generic;
using System.Xml;
using System.Threading.Tasks;

#endregion

namespace BlockBusters.Data {

    /// <summary>
    /// An enumerator that aids in distinguishing between
    /// the varying difficulties of the questions in the
    /// compilation.
    /// </summary>
    public enum QuestionDifficulty { 
        Easy, Medium, Hard
    }

    #region Structures

    /// <summary>
    /// A small convenient structure that holds all the 
    /// data surrounding a question and answer combination.
    /// </summary>
    public struct QA {
        public string category, question, A, B, C, D;
        public char answer, alpha;
        public QuestionDifficulty difficulty;
    }

    #endregion

    #region Objects

    public class QA_Compiler {

        #region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public QA_Compiler() {
            m_compilation = new List<QA>();
        }

        #endregion

        #region Declarations

        private List<QA> m_compilation;
        private XmlReader m_xmlReader;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current list of Question and Answer compilations.
        /// </summary>
        public List<QA> Compilation {
            get { return m_compilation; }
        }

        #endregion

        #region Functions

        /// <summary>
        /// Initialises an instance of XmlReader with a given XML file and DTD Parsing on.
        /// Run this function to set the reader to a new file prior to reading its data.
        /// </summary>
        /// <param name="xmlFile">
        /// A string representing a path to the XML file to be parsed.
        /// </param>
        /// <returns>
        /// Returns this instance of QA_Compiler to enable chaining.
        /// </returns>
        public QA_Compiler setReader(string xmlFile) {
            XmlReaderSettings settings = new XmlReaderSettings {
                IgnoreWhitespace = true,
                CloseInput = true,
                DtdProcessing = System.Xml.DtdProcessing.Parse
            };
            m_xmlReader = XmlReader.Create(xmlFile, settings);

            return this;
        }

        /// <summary>
        /// Reads all data from the stream and dumps it into the QA list.
        /// </summary>
        /// <returns>
        /// Returns this instance of QA_Compiler to enable chaining.
        /// </returns>
        public QA_Compiler readDataToList() {
            bool success = false;
            
            /* To store the question category, which is located
             * at the root of the document. */
            string category = string.Empty;
            
            /* Get the reader into position (the root node), grab
             * the category, and move into the first entry. */
            if (m_xmlReader.MoveToContent() == XmlNodeType.Element) 
                category = m_xmlReader.Name;

            // Now start the read loop
            while (m_xmlReader.Read()) { 
                // Temp QA variable to collect data
                QA tmp = new QA();

                if (m_xmlReader.Name.Equals("QA") && (m_xmlReader.NodeType == XmlNodeType.Element)) {
                    // Ensure category matches the root node
                    tmp.category = category;
                    // Move to the child (Question) Node
                    m_xmlReader.ReadToDescendant("Question");
                    // Get the answer attribute
                    tmp.answer = m_xmlReader.GetAttribute("answer")[0];
                    // Get the difficulty
                    string difficulty = m_xmlReader.GetAttribute("difficulty");
                    switch (difficulty) { 
                        case "easy":
                            tmp.difficulty = QuestionDifficulty.Easy;
                            break;
                        case "medium":
                            tmp.difficulty = QuestionDifficulty.Medium;
                            break;
                        case "hard":
                            tmp.difficulty = QuestionDifficulty.Hard;
                            break;
                        default:
                            break;
                    }
                    // Grab the alpha character
                    tmp.alpha = m_xmlReader.GetAttribute("alpha")[0];
                    // Get the question itself
                    tmp.question = m_xmlReader.ReadElementContentAsString();
                    // Move up to the next (Answers) node
                    m_xmlReader.MoveToElement();
                    /* Drop down a level to the children of Answers node and read all
                     * nodes, moving to the next node as the tree is traversed. */
                    m_xmlReader.ReadToDescendant("A");
                    tmp.A = m_xmlReader.ReadElementContentAsString();
                    m_xmlReader.MoveToElement();    // B
                    tmp.B = m_xmlReader.ReadElementContentAsString();
                    m_xmlReader.MoveToElement();    // C
                    tmp.C = m_xmlReader.ReadElementContentAsString();
                    m_xmlReader.MoveToElement();    // D
                    tmp.D = m_xmlReader.ReadElementContentAsString();
                    // Check for data retrieval consistency
                    success = QA_consistencyChk(tmp);

                    if (!success)
                        break;
                    else {
                        /* Finally add the question, it's answer, and all 
                         * other, relevant details to the compilation. */
                        m_compilation.Add(tmp);
                    }
                }
            }

            // We are finished with the XML Reader, lets close it down
            m_xmlReader.Close();

            return this;
        }

        /// <summary>
        /// Checks if the QA struct instance is complete.
        /// </summary>
        /// <param name="qa">
        /// Represents the QA structure.
        /// </param>
        /// <returns>
        /// True if each member has a value.
        /// False otherwise.
        /// </returns>
        private bool QA_consistencyChk(QA qa) {
            return (
                !string.IsNullOrEmpty(qa.A) &&
                !string.IsNullOrEmpty(qa.B) &&
                !string.IsNullOrEmpty(qa.C) &&
                !string.IsNullOrEmpty(qa.D) &&
                !string.IsNullOrEmpty(qa.category) &&
                !string.IsNullOrEmpty(qa.question) &&
                qa.answer != '\0');
        }

        /// <summary>
        /// Clears all items from the compilation.
        /// </summary>
        public void clearCompilation() {
            m_compilation.Clear();
        }

        /// <summary>
        /// Retrieves a random question from the compilation.
        /// </summary>
        /// <returns>
        /// Returns a random QA from the compilation.
        /// </returns>
        public QA getRandomQA() {
            Random r = new Random();
            return m_compilation[r.Next(0, m_compilation.Count)];
        }

        /// <summary>
        /// Retrieves a random question from the compilation ordered by 
        /// the answers first character.
        /// </summary>
        /// <param name="letter">
        /// Character represents the first character of the answers.
        /// </param>
        /// <returns>
        /// Returns a random QA from the compilation.
        /// </returns>
        public QA getRandomQA(char letter) {
            Random r = new Random();
            List<QA> lqa = m_compilation.FindAll(x => x.alpha.Equals(letter));
            
            return lqa[r.Next(0, lqa.Count)];
        }

        /// <summary>
        /// Retrieves a random question from the compilation
        /// ordered by letter and category.
        /// </summary>
        /// <param name="letter">
        /// Character represents the first character of the answers.
        /// </param>
        /// <param name="category">
        /// String represents the category tp search for on the compilation.
        /// </param>
        /// <returns>
        /// Returns a random QA from the compilation.
        /// </returns>
        public QA getRandomQA(char letter, string category) {
            Random r = new Random();
            List<QA> lsqa = m_compilation.FindAll(
                x => x.alpha.Equals(letter) && x.category.Equals(category));

            return lsqa[r.Next(0, lsqa.Count)];
        }

        /// <summary>
        /// Retrieves a random question from the compilation
        /// ordered by letter and difficulty.
        /// </summary>
        /// <param name="letter">
        /// Character represents the first character of the answers.
        /// </param>
        /// <param name="difficulty">
        /// Represents the difficulty enumerator.
        /// </param>
        /// <returns>
        /// Returns a random QA from the compilation.
        /// </returns>
        public QA getRandomQA(char letter, QuestionDifficulty difficulty) {
            Random r = new Random();
            List<QA> ldqa = m_compilation.FindAll(
                x => x.alpha.Equals(letter) && x.difficulty.Equals(difficulty));

            return ldqa[r.Next(0, ldqa.Count)];
        }

        /// <summary>
        /// Retrieves a random question from the compilation
        /// ordered by letter, category and difficulty.
        /// </summary>
        /// <param name="letter">
        /// Character represents the first character of the answers.
        /// </param>
        /// <param name="category">
        /// String represents the category tp search for on the compilation.
        /// </param>
        /// <param name="difficulty">
        /// Represents the difficulty enumerator.
        /// </param>
        /// <returns>
        /// Returns a random QA from the compilation.
        /// </returns>
        public QA getRandomQA(char letter, string category, QuestionDifficulty difficulty) {
            Random r = new Random();
            List<QA> lcdqa = m_compilation.FindAll(
                x => x.alpha.Equals(letter) && 
                    x.category.Equals(category) && 
                    x.difficulty.Equals(difficulty));

            return lcdqa[r.Next(0, lcdqa.Count)];
        }

        #endregion
    }

    #endregion
}