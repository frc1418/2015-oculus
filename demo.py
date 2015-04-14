from networktables import NetworkTabels 

NetworkTable.setIPAddress("localhost")
NetworkTable.setClientMode()
NetworkTable.initialize()
self.sd = NetworkTable.getTable("SmartDashboard")

class OculusDemoSimulator:
    def __init__(self):
        NetworkTable.setIPAddress("localhost")
        NetworkTable.setClientMode()
        NetworkTable.initialize()
        self.sd = NetworkTable.getTable("SmartDashboard")
        
        self.counter = 0
        
    def updateDistanceSensors(self):
        
        
    def run(self):
        
        while 1:
            
            if self.count == 145:
                self.counter = 0
            else:
                self.counter += 1
            

if __name__ == '__main__':
    demo = OculusDemoSimulator()
    demo.run()