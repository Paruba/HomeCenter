import * as React from 'react';
import { Line } from 'react-chartjs-2';
import { Chart as ChartJS } from 'chart.js/auto';

function TemperatureGraph({ data }) {
    let chartData;  // Declare chartData here
    const sortedData = [...data].sort((a, b) => new Date(a.time) - new Date(b.time));

    try {
        chartData = {
            labels: sortedData.map(item => new Date(item.time).toLocaleTimeString()),
            datasets: [
                {
                    label: 'Teplota (\u00B0C)',
                    data: sortedData.map(item => item.value),
                    fill: false,
                    borderColor: 'rgb(75, 192, 192)',
                    tension: 0.1
                }
            ]
        };
    } catch (error) {
        return <div>Error displaying temperature data.</div>;
    }

    return <Line data={chartData} style={{ width: '800px' }} />;
}

export default TemperatureGraph;
