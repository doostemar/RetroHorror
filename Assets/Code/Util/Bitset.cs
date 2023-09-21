using System;

public class Bitset
{
  ulong[] m_Backing;
  uint    m_WordCount;

  public Bitset( uint bits, ulong init_value = 0 )
  {
    m_WordCount = GetWordCount( bits );
    m_Backing = new ulong[ m_WordCount ];

    for ( uint i_word = 0; i_word < m_WordCount; ++i_word )
    {
      m_Backing[ i_word ] = 0;
    }
  }

  public static uint GetWordCount( uint bits )
  {
    return bits >> 6 + ( ( bits & 0x07 ) > 0 ? 1 : 0 );
  }

  public uint FindFirstFree()
  {
    unchecked
    {
      for ( uint i_word = 0; i_word < m_WordCount; ++i_word )
      {
        ulong word = m_Backing[ i_word ];
        if ( word != (ulong)-1 )
        {
          int bit = 0;
          while ( ( ( word >> ( 63 - bit ) ) & 1 ) != 0 )
          {
            bit++;
          }

          return (uint)(( i_word << 6 ) + bit);
        }
      }


      return (uint)-1;
    }
  }

  public void Set( uint idx )
  {
    m_Backing[ idx >> 6 ] |= ((ulong)1 << (63 - ((int)idx & 0x3f) ));
  }

  public void Unset( uint idx )
  {
    m_Backing[ idx >> 6 ] &= ~((ulong)1 << (63 - ((int)idx & 0x3f) ));
  }

  public bool GetState( uint idx )
  {
    return ( m_Backing[idx >> 6] & ~((ulong)1 << (63 - ((int)idx & 0x3f) ) ) ) != 0;
  }

  // return true to exit loop early
  //public delegate bool ForEachDelegate( int idx );
  public void ForEach( Func< uint, bool > cb )
  {
    for ( uint i_word = 0; i_word < m_WordCount; ++i_word )
    {
      if ( m_Backing[ i_word ] > 0 )
      {
        for ( int bit = 0; bit < 64; ++bit )
        {
          if ( ( m_Backing[ i_word ] & ((ulong)1 << (63 - bit ) ) ) != 0 )
          {
            if ( cb( (uint)(( i_word << 6 ) + bit )) )
            {
              return;
            }
          }
        }
      }
    }
  }
}
