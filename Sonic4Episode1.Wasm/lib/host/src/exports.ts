
export type Exports = {
    Program: {
        SetViewSize: (width: number, height: number) => void;
    };
    Microsoft: {
        Xna: {
            Framework: {
                Media: {
                    WasmMediaPlayer: {
                        ChangeState: (state: number) => void;
                    };
                };
            };
        },
        Devices: {
            Sensors: {
                Accelerometer: {
                    Invoke: (x: number, y: number, z: number) => void;
                }
            }
        }
    };
};
