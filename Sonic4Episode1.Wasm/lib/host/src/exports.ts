
export type Exports = {
    Program: {
        SetViewSize: (width: number, height: number) => void;
    };
    Microsoft: {
        Devices: {
            Sensors: {
                Accelerometer: {
                    Invoke: (x: number, y: number, z: number) => void;
                }
            }
        }
    };
};
