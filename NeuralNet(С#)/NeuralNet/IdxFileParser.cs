using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNet
{
    class IdxFileParser
    {
        private string m_strFilename;
        private int m_nMagicNumber;
        private int m_nDim;
        private int[] m_arrDim = new int[3];
        private int m_nItemSize;
        private bool m_fGood;

        public IdxFileParser(string filename, int dimX = 0, int dimY = 0, int dimZ = 0)
        {
            m_strFilename = filename;
            m_nItemSize = 1;
            m_nDim = 1;
            m_fGood = true;

            m_arrDim[0] = dimX;
            m_arrDim[1] = dimY;
            m_arrDim[2] = dimZ;

            if (dimY > 0)
            {
                ++m_nDim;
                m_nItemSize *= dimY;
            }
            if (dimZ > 0)
            {
                ++m_nDim;
                m_nItemSize *= dimZ;
            }

            m_nMagicNumber = (0x0800 | m_nDim);
            m_nMagicNumber = IPAddress.NetworkToHostOrder(m_nMagicNumber);

            if (File.Exists(filename))
            {
                //fStream = new FileStream(filename, FileMode.Open);
                BinaryReader reader = new BinaryReader(File.Open(m_strFilename, FileMode.Open));
                int magicNumToCheck = -1;
                magicNumToCheck = reader.ReadInt32();

                if (m_nMagicNumber != magicNumToCheck)
                {
                    m_fGood = false;
                }
                else
                { // check dimensions
                    int[] dimsToCheck = new int[3] { 0, 0, 0 };

                    for (int i = 0; i < m_nDim; i++)
                    {
                        dimsToCheck[i] = reader.ReadInt32();
                    }

                    for (int d = 0; d < m_nDim; ++d)
                    {
                        dimsToCheck[d] = IPAddress.NetworkToHostOrder(dimsToCheck[d]);

                        if (d == 0)
                        {
                            m_arrDim[0] = dimsToCheck[0];
                        }
                        else if (dimsToCheck[d] != m_arrDim[d])
                        {
                            m_fGood = false;
                            break;
                        }
                    }
                }
                reader.Close();
            }
            else
            { // file doesn't exist, create one:
                //fStream = new FileStream(m_strFilename, FileMode.Create);
                BinaryWriter writer = new BinaryWriter(File.Open(m_strFilename, FileMode.Create));
                if (writer != null)
                {
                    writer.Write(m_nMagicNumber);
                    for (int d = 0; d < m_nDim; ++d)
                    {
                        int dim = m_arrDim[d];
                        dim = IPAddress.NetworkToHostOrder(dim);
                        writer.Write(dim);
                    }
                    writer.Close();
                }
                else m_fGood = false;
            }
        }
	
	    public bool isGood() { return m_fGood; }

	    public int itemSize() { return m_nItemSize; }

	    public int dim(int idx) { return m_arrDim[idx]; }

	    public int readData(int index, ref Byte[] data, int maxSize = -1)
        {
            if (!m_fGood)
                return -1;
           
            int read = -1;

            if (File.Exists(m_strFilename))
            {

                //FileStream fStream = new FileStream(m_strFilename, FileMode.Open);
                //BinaryReader reader = new BinaryReader(File.Open(m_strFilename, FileMode.Open));
                BinaryReader reader = new BinaryReader(File.Open(m_strFilename, FileMode.Open));
                // check header
                int magicNumToCheck = -1;
                magicNumToCheck = reader.ReadInt32();
                if (m_nMagicNumber != magicNumToCheck)
                {
                    m_fGood = false;
                    reader.Close();
                    return -1;
                }

                // read item
                int offset = 4 + 4 * m_nDim + index * m_nItemSize;
                m_fGood = (offset == reader.BaseStream.Seek(offset, SeekOrigin.Begin)); 

                if (m_fGood)
                {
                    read = reader.Read(data, 0, m_nItemSize);
                    if (read != m_nItemSize)
                    {
                        return -1;
                    }
                }
                reader.Close();
            }
            return read;
        }

	    public int appendData(Byte[] data)
        {
            if (!m_fGood)
                return -1;
            
            int write = -1;

            if (File.Exists(m_strFilename))
            {
                BinaryReader reader = new BinaryReader(File.Open(m_strFilename, FileMode.Open));
                int magicNumToCheck = -1;
                magicNumToCheck = reader.ReadInt32();
                m_arrDim[0] = reader.ReadInt32();
                m_arrDim[0] = IPAddress.NetworkToHostOrder(m_arrDim[0]);
                reader.Close();

                BinaryWriter writer = new BinaryWriter(File.Open(m_strFilename, FileMode.Open));
                if (m_nMagicNumber != magicNumToCheck)
                {
                    m_fGood = false;
                    writer.Close();
                    return -1;
                }

                m_fGood = (writer.BaseStream.Length == writer.Seek(0, SeekOrigin.End));
                if (m_fGood)
                {
                
                    writer.Write(data);
                    writer.Flush();
                    write = 1;
                }
                // if we've successfully written data, update header:
                if (m_nItemSize > 0)
                {
                    m_arrDim[0]++;
                    int size = m_arrDim[0];
                    size = IPAddress.NetworkToHostOrder(size);
                    writer.Seek(4, SeekOrigin.Begin);
                    writer.Write(size);
                    writer.Flush();
                }
                writer.Close();
            }
            return write;
        }

    }
}
