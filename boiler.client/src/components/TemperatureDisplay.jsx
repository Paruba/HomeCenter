import React, { useState, useEffect } from 'react';
import Typography from '@mui/material/Typography';
import TemperatureGraph from './TemperatureGraph.jsx';
import TemperatureTable from './TemperatureTable.jsx';
import { useAuth } from './Identity/AuthContext.jsx';
import { useSearchParams } from 'react-router-dom';
import AuthorizedApi from '../framework/http/AuthorizedApi.jsx'
import GlobalKeyHandler from '../framework/GlobalKeyHandler.jsx';
import * as signalR from "@microsoft/signalr";
import Cookies from 'js-cookie';
import config from '../../config.js';
import Box from '@mui/material/Box';
import Container from '@mui/material/Container';

function TemperatureDisplay() {
    GlobalKeyHandler();
    const [searchParams] = useSearchParams();
    const thermometerId = searchParams.get('thermometerId');
    const { isLoggedIn, logout, isInRole } = useAuth();
    const [temperature, setTemperature] = useState(null);
    const [temperatureData, setTemperaturesData] = useState([]);
    const [page, setPage] = useState(0);
    const [rowsPerPage, setRowsPerPage] = useState(20);
    const [totalCount, setTotalCount] = useState(0);
    const [connection, setConnection] = useState(null);

    useEffect(() => {
        GetCurrentTemperature();
        getTemperaturesData(page, rowsPerPage);
    }, [page, rowsPerPage]);

    useEffect(() => {
        const newConnection = new signalR.HubConnectionBuilder()
            .withUrl(`${config.apiUrl}temperatureHub`, {
                accessTokenFactory: () => Cookies.get('AccessToken')
            })
            .configureLogging(signalR.LogLevel.Warning)
            .withAutomaticReconnect()
            .build();
        setConnection(newConnection);
        return () => {
            if (newConnection) {
                newConnection.stop();
            }
        }
    }, []);

    useEffect(() => {
        if (connection) {
            connection.start()
                .then(result => {
                    console.log('Connected!');

                    connection.on("ReceiveTemperature", (data) => {
                        setTemperature(data);
                        getTemperaturesData(page, rowsPerPage);
                    });
                })
                .catch(e => console.log('Connection failed: ', e));
        }
    }, [connection]);

    const handleChangePage = (event, newPage) => {
        setPage(newPage);
    };

    const handleChangeRowsPerPage = (event) => {
        setRowsPerPage(parseInt(event.target.value, 10));
        setPage(0);
    };

    if (!isLoggedIn) {
        return (
            <div>
                {"Vítejte, pro zobrazení dat se přihlašte."}
            </div>
        );
    }

    if (!temperatureData.length) {
        return <div>Žádná data...</div>;
    }

    return (
        <Container sx={{ mt: 20 }}>
            <Box>
                <Typography variant="h4">
                    {temperature !== null ? `${temperature.value} \u00B0C` : 'Loading...'}
                </Typography>
            </Box>
            <Box sx={{ mt: 10 }}>
                <TemperatureGraph data={temperatureData} />
            </Box>
            <Box sx={{ mt: 10 }}>
                <TemperatureTable
                    data={temperatureData}
                    page={page}
                    rowsPerPage={rowsPerPage}
                    count={totalCount}
                    onChangePage={handleChangePage}
                    onChangeRowsPerPage={handleChangeRowsPerPage}
                />
            </Box>
        </Container>
    );

    async function getTemperaturesData(page, rowsPerPage) {
        const take = rowsPerPage;
        const skip = page * take;
        const data = await AuthorizedApi.getJson(`temperature?skip=${skip}&take=${take}&thermometerId=${thermometerId}`);
        setTemperaturesData(data.items);
        setTotalCount(data.totalCount);
    }

    async function GetCurrentTemperature() {
        const data = await AuthorizedApi.getJson(`temperature/current?thermometerId=${thermometerId}`);
        setTemperature(data);
    }
}

export default TemperatureDisplay;
