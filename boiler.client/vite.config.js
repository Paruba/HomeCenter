import { fileURLToPath, URL } from 'node:url';

import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-react';
import fs from 'fs';
import path from 'path';
import child_process from 'child_process';

const baseFolder =
    process.env.USER_PATH !== undefined && process.env.USER_PATH !== ''
        ? `${process.env.USER_PATH}/https`
        : `/app/https`;

const certificateArg = process.argv.map(arg => arg.match(/--name=(?<value>.+)/i)).filter(Boolean)[0];
const certificateName = certificateArg ? certificateArg.groups.value : "localhost";

if (!certificateName) {
    console.error('Invalid certificate name. Run this script in the context of an npm/yarn script or pass --name=<<app>> explicitly.')
    process.exit(-1);
}

const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
    if (0 !== child_process.spawnSync('dotnet', [
        'dev-certs',
        'https',
        '--export-path',
        certFilePath,
        '--format',
        'Pem',
        '--no-password',
    ], { stdio: 'inherit', }).status) {
        throw new Error("Could not create certificate.");
    }
}

// https://vitejs.dev/config/
// https://localhost:7017/
// 5173
export default defineConfig({
    plugins: [plugin()],
    resolve: {
        alias: {
            '@': fileURLToPath(new URL('./src', import.meta.url))
        }
    },
    server: {
        proxy: {
            '^/temperature': {
                target: 'https://localhost:7017/',
                secure: false
            },
            '^/identity': {
                target: 'https://localhost:7017/',
                secure: false
            },
            '^/thermometer': {
                target: 'https://localhost:7017/',
                secure: false
            },
            '^/camera': {
                target: 'https://localhost:7017/',
                secure: false
            },
            '^/streamNotificationHub': {
                target: 'https://localhost:7017/',
                secure: false,
                ws: true
            },
            '^/face': {
                target: 'https://localhost:7017/',
                secure: false
            }
        },
        port: 8081,
        https: {
            key: fs.readFileSync(keyFilePath),
            cert: fs.readFileSync(certFilePath),
        }
    }
})
