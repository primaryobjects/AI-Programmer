//  All code copyright (c) 2003 Barry Lapthorn
//  Website:  http://www.lapthorn.net
//
//  Disclaimer:  
//  All code is provided on an "AS IS" basis, without warranty. The author 
//  makes no representation, or warranty, either express or implied, with 
//  respect to the code, its quality, accuracy, or fitness for a specific 
//  purpose. Therefore, the author shall not have any liability to you or any 
//  other person or entity with respect to any liability, loss, or damage 
//  caused or alleged to have been caused directly or indirectly by the code
//  provided.  This includes, but is not limited to, interruption of service, 
//  loss of data, loss of profits, or consequential damages from the use of 
//  this code.
//
//
//  $Author: barry $
//  $Revision: 1.1 $
//
//  $Id: GA.cs,v 1.1 2003/08/19 20:59:05 barry Exp $
//
//  Modified by Lionel Monnier 30th aug 2004
//  Modified by Kory Becker 01-Jan-2013 http://www.primaryobjects.com/kory-becker.aspx

#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
#endregion

namespace AIProgrammer.Types
{
    /// <summary>
	/// Summary description for Genome.
	/// </summary>
    [Serializable]
    public class Genome
    {
		public Genome()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		public Genome(int length)
		{
			m_length = length;
            m_genes = new double[length];
            CreateGenes();
		}
		public Genome(int length, bool createGenes)
		{
			m_length = length;
            m_genes = new double[length];
            if (createGenes)
				CreateGenes();
		}

        public Genome(ref double[] genes)
        {
			m_length = genes.Length;
            m_genes = new double[m_length];
            Array.Copy(genes, m_genes, m_length);
		}

        public Genome DeepCopy()
        {
            Genome g = new Genome(m_length, false);
            Array.Copy(m_genes, g.m_genes, m_length);
            return g;
        }

        private void CreateGenes()
		{
            for (int i = 0; i < m_genes.Length; i++)
                m_genes[i] = m_random.NextDouble();
        }

        public void Crossover(ref Genome genome2, out Genome child1, out Genome child2)
		{
			int pos = (int)(m_random.NextDouble() * (double)m_length);
			child1 = new Genome(m_length, false);
			child2 = new Genome(m_length, false);

            // Array.Copy is the same speed (fraction slower) than iterating over the array.
            /*// Copy first half for child1 with random genes, second half with parent genes.
            Array.Copy(m_genes, 0, child1.m_genes, 0, pos);
            Array.Copy(genome2.m_genes, pos, child1.m_genes, pos, m_length - pos);

            // Copy first half for child2 with parent genes, second half with random genes.
            Array.Copy(genome2.m_genes, 0, child2.m_genes, 0, pos);
            Array.Copy(m_genes, pos, child2.m_genes, pos, m_length - pos);*/

            for(int i = 0 ; i < m_length ; i++)
			{
				if (i < pos)
				{
					child1.m_genes[i] = m_genes[i];
					child2.m_genes[i] = genome2.m_genes[i];
				}
				else
				{
					child1.m_genes[i] = genome2.m_genes[i];
					child2.m_genes[i] = m_genes[i];
				}
            }
		}

        /// <summary>
        /// Mutation by insert, replace, delete.
        /// - An index is selected in the genome.
        /// - If inserting, a mutated bit is inserted at the position. The remaining bits are moved up by one index. The last bit is dropped off.
        /// - If replacing, a mutated bit is set at the position.
        /// - If deleting, all bits are shifted down at the position. A mutated bit is added at the end of the array.
        /// </summary>
        public void Mutate()
		{
            double r = m_random.NextDouble();
            if (r <= 0.333)
            {
                // Insertion mutation.
                // Get shift index.
                int mutationIndex = m_random.Next(m_genes.Length);

                // Make a copy of the current bit before we mutate it.
                double shiftBit = m_genes[mutationIndex];

                // Set random bit at mutation index.
                m_genes[mutationIndex] = m_random.NextDouble();

                // Bump bits up by 1.
                for (int i = mutationIndex + 1; i < m_length;  i++)
                {
                    double nextShiftBit = m_genes[i];

                    m_genes[i] = shiftBit;

                    shiftBit = nextShiftBit;
                }
            }
            else if (r <= 0.666)
            {
                // Deletion mutation.
                // Get deletion index.
                int mutationIndex = m_random.Next(m_genes.Length);

                // Bump bits down by 1.
                for (int i = mutationIndex; i < m_length - 1; i++)
                {
                    m_genes[i] = m_genes[i + 1];
                }

                // Add a new mutation bit at end of genome to replace the deleted one.
                m_genes[m_length - 1] = m_random.NextDouble();
            }
            else
            {
                // Replacement mutation.
                // Mutate bits.
                for (int pos = 0; pos < m_length; pos++)
                {
                    if (m_random.NextDouble() < m_mutationRate)
                    {
                        double mutation = m_random.NextDouble();
                        m_genes[pos] = mutation;
                    }
                }
            }
		}

		public double[] Genes()
		{
			return m_genes;
		}

		public void Output()
		{
            foreach (double valeur in m_genes)
            {
				System.Console.WriteLine("{0:F4}", valeur);
			}
			System.Console.Write("------\n");
		}

		public void GetValues(ref double[] values)
		{
			for (int i = 0 ; i < m_length ; i++)
				values[i] = m_genes[i];
		}

		public double[] m_genes;
        public int m_length;
        public double m_fitness;
        public int age;
		static Random m_random = new Random((int)DateTime.Now.Ticks);

        public static double m_mutationRate;

		public double Fitness
		{
			get
			{
				return m_fitness;
			}
			set
			{
				m_fitness = value;
			}
		}

		public static double MutationRate
		{
			get
			{
				return m_mutationRate;
			}
			set
			{
				m_mutationRate = value;
			}
		}

		public int Length
		{
			get
			{
				return m_length;
			}
		}
	}
}
