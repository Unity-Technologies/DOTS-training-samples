using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DATA
{
    public struct Histogram
    {
        public int binCount;
        public float[] values;
        public Histogram(int _binCount)
        {
            binCount = _binCount;
            values = new float[_binCount];
        }
        public Histogram(params float[] _values)
        {
            binCount = _values.Length;
            values = _values;
        }
        public void Set_Value(int _binIndex, float _newValue)
        {
            values[_binIndex] = _newValue;
        }
        public float Get_Value(int _binIndex)
        {
            return values[_binIndex];
        }
        public void UpdateNoise(float _rateA = 1, float _offsetA = 0, float _incrementA = 0f, float _rateB = 1, float _offsetB = 0, float _incrementB = 0)
        {
            for (int i = 0; i < binCount; i++)
            {
                Set_Value(i, Anim.PNoise(
                    // A
                    _rateA: _rateA,
                    _offsetA: _offsetA * i,
                    _incrementA: _incrementA * i,
                    // B
                    _rateB: _rateB,
                    _offsetB: _offsetB * i,
                    _incrementB: _incrementB * i
                ));
            }
        }
    }
    public struct Graph
    {
        public int binCount;
        public Color[] colours;
        public float[] values;

        public Graph(int _binCount)
        {
            binCount = _binCount;
            values = new float[_binCount];
            colours = new Color[_binCount];
        }
        public void Set_Colour(int _index, Color _col)
        {
            colours[_index] = _col;
        }
        public Color Get_Colour(int _index)
        {
            return colours[_index];
        }
        public void Set_Value(int _index, float _value)
        {
            values[_index] = _value;
        }
        public float Get_Value(int _index)
        {
            return values[_index];
        }
        public void UpdateNoise(float _rateA = 1.1f, float _offsetA = 0.5f, float _rateB = 1.2f, float _offsetB = 0.75f)
        {
            for (int i = 0; i < binCount; i++)
            {
                Set_Value(i, Anim.PNoise(_rateA, _rateB, _offsetA * i, _offsetB * i));
            }
        }
    }
    public struct Spline
    {
        public Vector2[] points;
        public Color colour;
        public float tension;

        public Spline(Vector2[] _points, Color _colour, float _tension, bool _drawPoints = false, int _pointSides = 4)
        {
            points = _points;
            colour = _colour;
            tension = _tension;
        }
        public void Draw()
        {
            // draw points with tension
            // draw points if required
        }
    }
    public class Partition
    {
        public int index;
        public float start;
        public float share;
        public Color colour;
        public Partition(float _start, float _share, Color _col, int _index = 0)
        {
            start = _start;
            share = _share;
            colour = _col;
            index = _index;
        }
        public Partition(float _share)
        {
            share = _share;
            start = 0;
            colour = Color.red;
            index = 0;
        }

    }
    public class Partitions
    {
        public static Partition[] GetRandomPartitions(int _count)
        {
            Partition[] _list = new Partition[_count];
            float used = 0;
            for (int i = 0; i < _count; i++)
            {
                float share = Random.Range(0, 1f-used);
                _list[i] = new Partition(share);
                used += share;
            }
            // bloat a random partition to fill all space
            float _leftOver = 1f - used;
            Partition _RAND = _list[Mathf.FloorToInt(Random.Range(0, _count - 1))];
            _RAND.share = _RAND.share + _leftOver;
            return _list;
        }

        public Partition[] partitions;
        public int count;
        public float activeArea;
        public float gutterRatio;
        public float gutter;
        public Partitions(float _gutterRatio = HUD.DEFAULT_GUTTER_RATIO, params Partition[] _partitions){
            Init(_gutterRatio, _partitions);
        }
        public Partitions(int  _count = 10, float _gutterRatio = HUD.DEFAULT_GUTTER_RATIO)
        {
            Init(_gutterRatio, GetRandomPartitions(_count));
        }
        void Init(float _gutterRatio, Partition[] _partitions){
            gutterRatio = _gutterRatio;
            partitions = _partitions;
            count = partitions.Length;
            activeArea = _gutterRatio;
            float _START_COUNTER = 0;
            gutter = ((1f / (count)) * gutterRatio) / (count - 1);
            for (int i = 0; i < count; i++)
            {
                Partition _P = Get(i);
                _P.index = i;
                _P.start = _START_COUNTER + (i * gutter);
                _START_COUNTER += _P.share;
            }
        }
        public Partition Get(int _index){

            return partitions[_index];
        }
        public void Set_Share(int _index, float _value){
            Partition _P = Get(_index);
            float _PREV_SHARE = _P.share;
            float _DIFF = _PREV_SHARE - _value;
            _P.share = Mathf.Clamp01(_value);
            float _CHANGE_PER_PARTITION = (_DIFF / (count - 1));

            float _SHARE_COUNT = 0;
            for (int i = 0; i < count; i++)
            {
                Partition _TEMP_P = Get(i);

                if(_TEMP_P != _P){
                    _TEMP_P.share = Mathf.Clamp01(_TEMP_P.share += _CHANGE_PER_PARTITION);
                }
                _TEMP_P.start = _SHARE_COUNT + (i * gutter);
                _SHARE_COUNT += _TEMP_P.share;
            }
        }
        public void AddShare(int  _index, float _add){
            Partition _P = Get(_index);
            Set_Share(_index, _P.share + _add);
        }
        public void AddRandom(int _index, float _range=0.001f, float _rateA=1f, float _rateB=1.2f, float _offsetA=1f, float _offsetB=1.5f, float _incrementA=0.01f, float _incrementB=0.05f){
            AddShare(_index, Anim.PNoise(_rateA, _rateB, _offsetA, _offsetB, _incrementA, _incrementB, -_range, _range));
        }
        public void AddRandom(float _range = 0.001f, float _rateA = 1f, float _rateB = 1.2f, float _offsetA = 1f, float _offsetB = 1.5f, float _incrementA = 0.01f, float _incrementB = 0.05f)
        {
            for (int i = 0; i < count; i++)
            {
                AddShare(i, Anim.PNoise(_rateA, _rateB, _offsetA*i, _offsetB*i, _incrementA*i, _incrementB*i, -_range, _range));
            }
        }
        public void Set_Start(int _index, float _value){
            Get(_index).start =  _value + (_index * gutter);
        }
        public float Get_Start(int _index){
            return partitions[_index].start;
        }
        public float Get_Share(int _index)
        {
            return partitions[_index].share * activeArea;
        }
        public Color Get_Colour(int _index)
        {
            return partitions[_index].colour;
        }
        public void Set_Colour(int _index, Color _col)
        {
            partitions[_index].colour = _col;
        }
        public void ColourSpread(Color _col_start, Color _col_end){
            for (int i = 0; i < count; i++)
            {
                Set_Colour(i,Color.Lerp(_col_start, _col_end, (float)i / count));
            }
        }
        public void ColourByShare(Color _col_start, Color _col_end)
        {
            for (int i = 0; i < count; i++)
            {
                Partition _P = Get(i);
                _P.colour = (Color.Lerp(_col_start, _col_end, _P.share));
            }
        }
    }

    public class DataSprawl
    {

        List<BitArray> rows;
        int rowLength, totalRows, totalCells, currentRow, currentCell, timer_writeCell, currentFrame, minDuration, maxDuration, framesPerTick, cellRate;
        float durationFactor_MIN, durationFactor_MAX;
        public DataSprawl(int _totalRows, int _rowLength, int _framesPerTick = 2, int _cellRate = 1, float _durationFactor_MIN = 1f, float _durationFactor_MAX = 3f)
        {
            totalRows = _totalRows;
            rowLength = _rowLength;
            totalCells = totalRows * rowLength;
            rows = new List<BitArray>(totalRows);

            durationFactor_MIN = _durationFactor_MIN;
            durationFactor_MAX = _durationFactor_MAX;
            framesPerTick = _framesPerTick;
            minDuration = Mathf.FloorToInt(_cellRate * durationFactor_MIN);
            maxDuration = Mathf.FloorToInt(_cellRate * durationFactor_MAX);
            cellRate = _cellRate;
            timer_writeCell = 0;
            currentFrame = 0;
            currentRow = 0;
            currentCell = 0;

            InitRows();
            ResetTimer_WRITE();
        }
        private void ResetTimer_WRITE()
        {
            int _WRITE_COUNT = Random.Range(1, 10);
            timer_writeCell = Mathf.FloorToInt(Random.Range(minDuration, maxDuration));
            for (int i = 0; i < _WRITE_COUNT; i++)
            {
                NextFrame();
                rows[currentRow].Set(currentCell, true);
            }

            currentFrame += _WRITE_COUNT * cellRate;
        }
        private void InitRows()
        {
            rows = new List<BitArray>(totalRows);
            for (int i = 0; i < totalRows; i++)
            {
                rows.Add(new BitArray(rowLength, false));
            }
        }
        public void ClearRow(int _rowIndex)
        {
            BitArray _CELLS = rows[_rowIndex];
            for (int i = 0; i < rowLength; i++)
            {
                _CELLS.Set(i, false);
            }
        }
        void NextFrame()
        {
            currentFrame++;
            int _RAW_FRAME = Mathf.FloorToInt(currentFrame / (float)cellRate) % totalCells;
            currentCell = _RAW_FRAME % rowLength;

            int _TEST_NEW_ROW = Mathf.FloorToInt(_RAW_FRAME / (float)rowLength);
            if (currentRow != _TEST_NEW_ROW)
            {
                ClearRow((currentRow + 1) % totalRows);
                currentRow = _TEST_NEW_ROW;
            }
            timer_writeCell--;
        }
        public void Update()
        {
            for (int i = 0; i < framesPerTick; i++)
            {
                NextFrame();
            }
            if (timer_writeCell < 0)
            {
                ResetTimer_WRITE();
            }
        }
        public void Draw(float _x, float _y, float _w, float _h, Color _col, float _gutterRatio = HUD.DEFAULT_GUTTER_RATIO)
        {
            float _ACTIVE_AREA = _h * _gutterRatio;
            float _GUTTER = (_h - _ACTIVE_AREA) / ((totalRows - 1));
            float _ROW_HEIGHT = _ACTIVE_AREA / totalRows;

            for (int i = 0; i < totalRows; i++)
            {
                int _ROW_INDEX = ((currentRow + totalRows) - i) % totalRows;
                BitArray _ROW_CELLS = rows[_ROW_INDEX];
                GL_DRAW.Draw_RECT_CELLS(_x, _y + (i * _ROW_HEIGHT) + (i * _GUTTER), _w, _ROW_HEIGHT, _ROW_CELLS, _col, 1);
            }
        }
    }
    public class VALUES
    {

        public static float[] RandomValues_01(int _count)
        {
            float[] _RESULT = new float[_count];
            for (int i = 0; i < _count; i++)
            {
                _RESULT[i] = Random.value;
            }
            return _RESULT;
        }
        public static float[] RandomValues(int _count, float _min, float _max)
        {
            float[] _RESULT = new float[_count];
            float _RANGE = _max - _min;
            for (int i = 0; i < _count; i++)
            {
                _RESULT[i] = _min + (Random.value * _RANGE);
            }
            return _RESULT;
        }
        public static float[] RandomValues_NOISE(int _count, float _rateA = 0.1f, float _rateB = 0.2f, float _offset_MAIN = 0)
        {
            float[] _RESULT = new float[_count];
            for (int i = 0; i < _count; i++)
            {
                _RESULT[i] = Mathf.PerlinNoise(_offset_MAIN + (i * _rateA), _offset_MAIN + (i * _rateB));
            }
            return _RESULT;
        }
        public static float[] RandomValues_NOISE_TIME(int _count, float _rateA = 1, float _rateB = 1.1f, float _offsetA = 0.01f, float _offsetB = 0.025f, float _incrementA = 0.025f, float _incrementB = 0.01f)
        {
            float[] _RESULT = new float[_count];
            for (int i = 0; i < _count; i++)
            {
                _RESULT[i] = Anim.PNoise(_rateA, _rateB, _offsetA * i, _offsetB * i, _incrementA * i, _incrementB * i);
            }
            return _RESULT;
        }

        public static Color[] ColourGradient(int _stages, Color _colA, Color _colB)
        {
            Color[] _RESULT = new Color[_stages];
            for (int i = 0; i < _stages; i++)
            {
                _RESULT[i] = Color.Lerp(_colA, _colB, (float)i / _stages);
            }
            return _RESULT;
        }
    }
}
