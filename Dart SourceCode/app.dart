import 'package:flutter/material.dart';
import 'package:firebase_auth/firebase_auth.dart';
import 'package:cloud_firestore/cloud_firestore.dart';
import 'login.dart';
import 'home.dart';
import 'profile_setup.dart';

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  Future<Widget> _decideStartPage(User user) async {
    final doc = await FirebaseFirestore.instance
        .collection("users")
        .doc(user.uid)
        .get();

    if (doc.exists && doc.data()?["profileCompleted"] == true) {
      return const HomeScreen(); // profile already set
    } else {
      return const ProfileSetupPage(); // force profile setup
    }
  }

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Nichelin App',
      debugShowCheckedModeBanner: false,
      theme: ThemeData(
        colorScheme: ColorScheme.fromSeed(seedColor: Colors.deepPurple),
        useMaterial3: true,
      ),
      home: StreamBuilder<User?>(
        stream: FirebaseAuth.instance.authStateChanges(),
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const Scaffold(
              body: Center(child: CircularProgressIndicator()),
            );
          }

          if (snapshot.hasData) {
            // User is logged in → check profile
            return FutureBuilder<Widget>(
              future: _decideStartPage(snapshot.data!),
              builder: (context, futureSnapshot) {
                if (futureSnapshot.connectionState == ConnectionState.waiting) {
                  return const Scaffold(
                    body: Center(child: CircularProgressIndicator()),
                  );
                }
                return futureSnapshot.data!;
              },
            );
          }

          // User not logged in → login page
          return const CustomLoginPage();
        },
      ),
    );
  }
}
