import html from '../utils/index.js';
import Chart from '../components/chart.js';

const components = { Chart };

const template = html`<div
  style="margin: 1em auto;width:500px;"
  v-for="item in charts"
>
  <chart
    :options="item"
    height="280px"
  />
</div>`;

function setup() {
  const charts = [
    {
      title: {
        text: '基础折线图',
      },
      xAxis: {
        type: 'category',
        data: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'],
      },
      yAxis: {
        type: 'value',
      },
      series: [
        {
          data: [150, 230, 224, 218, 135, 147, 260],
          type: 'line',
        },
      ],
    },
    {
      title: {
        text: '基础柱状图',
      },
      xAxis: {
        type: 'category',
        data: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'],
      },
      yAxis: {
        type: 'value',
      },
      series: [
        {
          data: [120, 200, 150, 80, 70, 110, 130],
          type: 'bar',
        },
      ],
    },
    {
      title: {
        text: '饼图',
        left: 'center',
      },
      tooltip: {
        trigger: 'item',
      },
      legend: {
        orient: 'vertical',
        left: 'left',
      },
      series: [
        {
          name: 'Access From',
          type: 'pie',
          radius: '50%',
          data: [
            { value: 1048, name: 'Search' },
            { value: 735, name: 'Direct' },
            { value: 580, name: 'Email' },
            { value: 484, name: 'Union' },
            { value: 300, name: 'Video' },
          ],
          emphasis: {
            itemStyle: {
              shadowBlur: 10,
              shadowOffsetX: 0,
              shadowColor: 'rgba(0, 0, 0, 0.5)',
            },
          },
        },
      ],
    },
    {
      title: {
        text: '基础散点图',
      },
      xAxis: {},
      yAxis: {},
      series: [
        {
          symbolSize: 10,
          data: [
            [10.0, 8.04],
            [8.07, 6.95],
            [13.0, 7.58],
            [9.05, 8.81],
            [11.0, 8.33],
            [14.0, 7.66],
            [13.4, 6.81],
            [10.0, 6.33],
            [14.0, 8.96],
            [12.5, 6.82],
            [9.15, 7.2],
            [11.5, 7.2],
            [3.03, 4.23],
            [12.2, 7.83],
            [2.02, 4.47],
            [1.05, 3.33],
            [4.05, 4.96],
            [6.03, 7.24],
            [12.0, 6.26],
            [12.0, 8.84],
            [7.08, 5.82],
            [5.02, 5.68],
          ],
          type: 'scatter',
        },
      ],
    },
    {
      title: {
        text: '基础 K 线图',
      },
      xAxis: {
        data: ['2017-10-24', '2017-10-25', '2017-10-26', '2017-10-27'],
      },
      yAxis: {},
      series: [
        {
          type: 'candlestick',
          data: [
            [20, 34, 10, 38],
            [40, 35, 30, 50],
            [31, 38, 33, 44],
            [38, 15, 5, 42],
          ],
        },
      ],
    },
    {
      title: {
        text: '基础雷达图',
      },
      legend: {
        data: ['Allocated Budget', 'Actual Spending'],
      },
      radar: {
        // shape: 'circle',
        indicator: [
          { name: 'Sales', max: 6500 },
          { name: 'Administration', max: 16000 },
          { name: 'Information Technology', max: 30000 },
          { name: 'Customer Support', max: 38000 },
          { name: 'Development', max: 52000 },
          { name: 'Marketing', max: 25000 },
        ],
      },
      series: [
        {
          name: 'Budget vs spending',
          type: 'radar',
          data: [
            {
              value: [4200, 3000, 20000, 35000, 50000, 18000],
              name: 'Allocated Budget',
            },
            {
              value: [5000, 14000, 28000, 26000, 42000, 21000],
              name: 'Actual Spending',
            },
          ],
        },
      ],
    },
    {
      title: { text: '基础矩形树图' },
      series: [
        {
          type: 'treemap',
          data: [
            {
              name: 'nodeA',
              value: 10,
              children: [
                {
                  name: 'nodeAa',
                  value: 4,
                },
                {
                  name: 'nodeAb',
                  value: 6,
                },
              ],
            },
            {
              name: 'nodeB',
              value: 20,
              children: [
                {
                  name: 'nodeBa',
                  value: 20,
                  children: [
                    {
                      name: 'nodeBa1',
                      value: 20,
                    },
                  ],
                },
              ],
            },
          ],
        },
      ],
    },
    {
      title: { text: '基础平行坐标' },
      parallelAxis: [
        { dim: 0, name: 'Price' },
        { dim: 1, name: 'Net Weight' },
        { dim: 2, name: 'Amount' },
        {
          dim: 3,
          name: 'Score',
          type: 'category',
          data: ['Excellent', 'Good', 'OK', 'Bad'],
        },
      ],
      series: {
        type: 'parallel',
        lineStyle: {
          width: 4,
        },
        data: [
          [12.99, 100, 82, 'Good'],
          [9.99, 80, 77, 'OK'],
          [20, 120, 60, 'Excellent'],
        ],
      },
    },
    {
      title: {
        text: '漏斗图',
      },
      tooltip: {
        trigger: 'item',
        formatter: '{a} <br/>{b} : {c}%',
      },
      toolbox: {
        feature: {
          dataView: { readOnly: false },
          restore: {},
          saveAsImage: {},
        },
      },
      legend: {
        data: ['Show', 'Click', 'Visit', 'Inquiry', 'Order'],
      },
      series: [
        {
          name: 'Funnel',
          type: 'funnel',
          left: '10%',
          top: 60,
          bottom: 60,
          width: '80%',
          min: 0,
          max: 100,
          minSize: '0%',
          maxSize: '100%',
          sort: 'descending',
          gap: 2,
          label: {
            show: true,
            position: 'inside',
          },
          labelLine: {
            length: 10,
            lineStyle: {
              width: 1,
              type: 'solid',
            },
          },
          itemStyle: {
            borderColor: '#fff',
            borderWidth: 1,
          },
          emphasis: {
            label: {
              fontSize: 20,
            },
          },
          data: [
            { value: 60, name: 'Visit' },
            { value: 40, name: 'Inquiry' },
            { value: 20, name: 'Order' },
            { value: 80, name: 'Click' },
            { value: 100, name: 'Show' },
          ],
        },
      ],
    },
  ];
  return {
    charts,
  };
}

export default {
  components,
  template,
  setup,
};
